using EnergyAPI.Models;

namespace EnergyAPI.Services;

internal static class EnergyConstants
{
    public static readonly string[] CleanFuels = ["biomass", "nuclear", "hydro", "wind", "solar"];

    /// <summary>
    /// Sums the percentage share of all clean fuels (biomass, nuclear, hydro, wind, solar) in a generation mix.
    /// </summary>
    public static double CleanPercentage(IEnumerable<FuelMix> mix) =>
        mix.Where(f => CleanFuels.Contains(f.Fuel)).Sum(f => f.Perc);
}
