using EnergyAPI.Models;
using EnergyAPI.Services;
using NSubstitute;

namespace EnergyTests;

public class ChargingWindowServiceTests
{
    private readonly ICarbonIntensity _mock;
    private readonly ChargingWindowService _service;

    public ChargingWindowServiceTests()
    {
        _mock = Substitute.For<ICarbonIntensity>();
        _service = new ChargingWindowService(_mock);
    }

    [Fact]
    public async Task GetOptimalWindowAsync_ReturnsWindowWithHighestAverageCleanPercentage()
    {
        // hours=1 -> intervals = 2 (half-hour slots)
        var start = DateTime.Today;

        _mock.GetGenerationAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new GenerationResponse(
            [
                new GenerationInterval(start, start.AddMinutes(30),
                    [new FuelMix("wind", 10), new FuelMix("gas", 90)]),       // window 0: avg(10,20)=15
                new GenerationInterval(start.AddMinutes(30), start.AddHours(1),
                    [new FuelMix("wind", 20), new FuelMix("gas", 80)]),
                new GenerationInterval(start.AddHours(1), start.AddHours(1).AddMinutes(30),
                    [new FuelMix("wind", 80), new FuelMix("gas", 20)]),        // window 2: avg(80,90)=85 <- best
                new GenerationInterval(start.AddHours(1).AddMinutes(30), start.AddHours(2),
                    [new FuelMix("wind", 90), new FuelMix("gas", 10)]),
            ]));

        var result = await _service.GetOptimalWindowAsync(hours: 1);

        Assert.Equal(start.AddHours(1), result.Start);
        Assert.Equal(start.AddHours(2), result.End);
        Assert.Equal(85, result.AverageCleanEnergy, precision: 3);
    }

    [Fact]
    public async Task GetOptimalWindowAsync_RequestsCorrectDateRange()
    {
        var start = DateTime.Today;

        _mock.GetGenerationAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new GenerationResponse(
            [
                new GenerationInterval(start, start.AddMinutes(30),
                    [new FuelMix("wind", 50), new FuelMix("gas", 50)]),
                new GenerationInterval(start.AddMinutes(30), start.AddHours(1),
                    [new FuelMix("wind", 50), new FuelMix("gas", 50)]),
            ]));

        var today = DateTime.UtcNow.Date;

        await _service.GetOptimalWindowAsync(hours: 1);

        await _mock.Received(1)
            .GetGenerationAsync(today.AddDays(1), today.AddDays(3));
    }
    
    [Fact]
    public async Task GetOptimalWindowAsync_OnlyCountsConfiguredCleanFuels()
    {
        // solar is clean, coal is not
        var start = DateTime.UtcNow.Date;

        _mock.GetGenerationAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new GenerationResponse(
            [
                new GenerationInterval(start, start.AddMinutes(30),
                    [new FuelMix("solar", 30), new FuelMix("coal", 70)]),
                new GenerationInterval(start.AddMinutes(30), start.AddHours(1),
                    [new FuelMix("solar", 40), new FuelMix("coal", 60)]),
            ]));

        var result = await _service.GetOptimalWindowAsync(hours: 1);

        // average of 30 and 40, ignoring coal entirely
        Assert.Equal(35, result.AverageCleanEnergy, precision: 3);
    }

    [Fact]
    public async Task GetOptimalWindowAsync_WhenTied_PicksFirstOccurringWindow()
    {
        // all windows identical -> bestIndex stays 0 (loop uses '>' not '>=')
        var start = DateTime.Today;

        _mock.GetGenerationAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new GenerationResponse(
            [
                new GenerationInterval(start, start.AddMinutes(30),
                    [new FuelMix("wind", 50), new FuelMix("gas", 50)]),
                new GenerationInterval(start.AddMinutes(30), start.AddHours(1),
                    [new FuelMix("wind", 50), new FuelMix("gas", 50)]),
                new GenerationInterval(start.AddHours(1), start.AddHours(1).AddMinutes(30),
                    [new FuelMix("wind", 50), new FuelMix("gas", 50)]),
                new GenerationInterval(start.AddHours(1).AddMinutes(30), start.AddHours(2),
                    [new FuelMix("wind", 50), new FuelMix("gas", 50)]),
            ]));

        var result = await _service.GetOptimalWindowAsync(hours: 1);

        Assert.Equal(start, result.Start);
        Assert.Equal(start.AddHours(1), result.End);
    }
}
