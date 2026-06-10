using EnergyAPI.Models;

namespace EnergyAPI.Services;

public class ChargingWindowService : IChargingWindowService
{
    private readonly ICarbonIntensity _carbonIntensity;

    public ChargingWindowService(ICarbonIntensity carbonIntensity)
    {
        _carbonIntensity = carbonIntensity;
    }

    public async Task<OptimalChargingWindow> GetOptimalWindowAsync(int hours)
    {
        var today = DateTime.UtcNow.Date;
        var data = await _carbonIntensity.GetGenerationAsync(today.AddDays(1), today.AddDays(3));
        var windowSize = hours * 2;

        double bestCleanPercentage = 0;
        int bestIndex = 0;

        for (int i = 0; i <= data.Data.Count - windowSize; i++)
        {
            var windowCleanPercentage = data.Data
                .Skip(i)
                .Take(windowSize)
                .Select(d => EnergyConstants.CleanPercentage(d.Generationmix))
                .Average();

            if (windowCleanPercentage > bestCleanPercentage)
            {
                bestCleanPercentage = windowCleanPercentage;
                bestIndex = i;
            }
        }

        return new OptimalChargingWindow(
            data.Data[bestIndex].From,
            data.Data[bestIndex + windowSize - 1].To,
            bestCleanPercentage);
    }
}
