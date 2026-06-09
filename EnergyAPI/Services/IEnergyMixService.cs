using EnergyAPI.Models;

namespace EnergyAPI.Services;

public interface IEnergyMixService
{
    Task<List<DailyEnergyMix>> GetThreeDayMixAsync();
}
