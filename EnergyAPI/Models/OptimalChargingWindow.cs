namespace EnergyAPI.Models;

public record OptimalChargingWindow(DateTime Start, DateTime End, double AverageCleanEnergy);
