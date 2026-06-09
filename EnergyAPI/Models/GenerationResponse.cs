namespace EnergyAPI.Models;

public record GenerationResponse(List<GenerationInterval> Data);
public record GenerationInterval(DateTime From, DateTime To, List<FuelMix> Generationmix);
public record FuelMix(string Fuel, double Perc);