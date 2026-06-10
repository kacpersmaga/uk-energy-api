using EnergyAPI.Models;
using EnergyAPI.Services;
using NSubstitute;

namespace EnergyTests;

public class EnergyMixServiceTests
{
    private readonly ICarbonIntensity _mock;
    private readonly EnergyMixService _service;

    public EnergyMixServiceTests()
    {
        _mock = Substitute.For<ICarbonIntensity>();
        _service = new EnergyMixService(_mock);
    }

    [Fact]
    public async Task GetThreeDayMix_GroupsByDay()
    {
        _mock.GetGenerationAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new GenerationResponse(
            [
                new GenerationInterval(DateTime.Today, DateTime.Today.AddMinutes(30),
                    [new FuelMix("solar", 20), new FuelMix("gas", 80)]),
                new GenerationInterval(DateTime.Today.AddMinutes(30), DateTime.Today.AddHours(1),
                    [new FuelMix("solar", 40), new FuelMix("gas", 60)]),
                new GenerationInterval(DateTime.Today.AddDays(1), DateTime.Today.AddDays(1).AddMinutes(30),
                    [new FuelMix("wind", 50), new FuelMix("gas", 50)]),
            ]));

        var result = await _service.GetThreeDayMixAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetThreeDayMix_AveragesFuelsWithinDay()
    {
        // solar: 20 and 40 -> average 30
        _mock.GetGenerationAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new GenerationResponse(
            [
                new GenerationInterval(DateTime.Today, DateTime.Today.AddMinutes(30),
                    [new FuelMix("solar", 20)]),
                new GenerationInterval(DateTime.Today.AddMinutes(30), DateTime.Today.AddHours(1),
                    [new FuelMix("solar", 40)]),
            ]));

        var result = await _service.GetThreeDayMixAsync();

        var solar = result[0].AverageMix.Single(f => f.Fuel == "solar");
        Assert.Equal(30, solar.Perc);
    }

    [Fact]
    public async Task GetThreeDayMix_CalculatesCleanPercentage()
    {
        // wind 50% + gas 50% -> clean = 50%
        _mock.GetGenerationAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new GenerationResponse(
            [
                new GenerationInterval(DateTime.Today, DateTime.Today.AddMinutes(30),
                    [new FuelMix("wind", 50), new FuelMix("gas", 50)]),
            ]));

        var result = await _service.GetThreeDayMixAsync();

        Assert.Equal(50, result[0].CleanEnergyPercentage);
    }

    [Fact]
    public async Task GetThreeDayMix_WhenNoCleanFuels_ReturnsZeroCleanPercentage()
    {
        _mock.GetGenerationAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new GenerationResponse(
            [
                new GenerationInterval(DateTime.Today, DateTime.Today.AddMinutes(30),
                    [new FuelMix("gas", 100)]),
            ]));

        var result = await _service.GetThreeDayMixAsync();

        Assert.Equal(0, result[0].CleanEnergyPercentage);
    }
}