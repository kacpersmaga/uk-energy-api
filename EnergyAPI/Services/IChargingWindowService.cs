using EnergyAPI.Models;

namespace EnergyAPI.Services;

public interface IChargingWindowService
{
    Task<OptimalChargingWindow> GetOptimalWindowAsync(int hours);
}
