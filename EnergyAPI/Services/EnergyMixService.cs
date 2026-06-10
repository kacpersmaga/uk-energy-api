using EnergyAPI.Models;

namespace EnergyAPI.Services;

public class EnergyMixService : IEnergyMixService
{
    private readonly ICarbonIntensity _carbonIntensity;

    public EnergyMixService(ICarbonIntensity carbonIntensity)
    {
        _carbonIntensity = carbonIntensity;
    }

    public async Task<List<DailyEnergyMix>> GetThreeDayMixAsync()
    {
        var today = DateTime.UtcNow.Date;
        
        var data = await _carbonIntensity.GetGenerationAsync(today, today.AddDays(3));

        return data.Data
            .GroupBy(interval => DateOnly.FromDateTime(interval.From))
            .Select(dayGroup =>
            {
                var fuels = dayGroup
                    .SelectMany(i => i.Generationmix)
                    .GroupBy(f => f.Fuel)
                    .Select(g => new FuelMix(g.Key, g.Average(f => f.Perc)))
                    .ToList();

                var cleanPercentage = EnergyConstants.CleanPercentage(fuels);

                return new DailyEnergyMix(dayGroup.Key, fuels, cleanPercentage);
            })
            .Take(3)
            .ToList();
    }
}
