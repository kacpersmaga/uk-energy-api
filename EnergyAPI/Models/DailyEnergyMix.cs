namespace EnergyAPI.Models;

public record DailyEnergyMix(
    DateOnly Date,
    List<FuelMix> AverageMix,
    double CleanEnergyPercentage);