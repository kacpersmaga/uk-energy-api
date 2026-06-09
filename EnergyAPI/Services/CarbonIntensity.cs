using System.Text.Json;
using EnergyAPI.Models;

namespace EnergyAPI.Services;

public class CarbonIntensity : ICarbonIntensity
{
    private readonly HttpClient _client;

    public CarbonIntensity(HttpClient client)
    {
        _client = client;
    }

    public async Task<GenerationResponse> GetGenerationAsync(DateTime from, DateTime to)
    {
        var url = $"https://api.carbonintensity.org.uk/generation/{from:yyyy-MM-ddTHH:mmZ}/{to:yyyy-MM-ddTHH:mmZ}";

        HttpResponseMessage response;
        try
        {
            response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Carbon Intensity API request failed: {ex.Message}", ex, ex.StatusCode);
        }

        var body = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<GenerationResponse>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
}
