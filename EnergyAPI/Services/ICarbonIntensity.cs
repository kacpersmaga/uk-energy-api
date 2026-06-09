using EnergyAPI.Models;

namespace EnergyAPI.Services;

public interface ICarbonIntensity
{
    Task<GenerationResponse> GetGenerationAsync(DateTime from, DateTime to);
    
}