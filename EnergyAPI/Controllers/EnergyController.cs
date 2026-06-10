using EnergyAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnergyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnergyController : ControllerBase
{
    private readonly IEnergyMixService _energyMixService;
    private readonly IChargingWindowService _chargingWindowService;

    public EnergyController(IEnergyMixService energyMixService, IChargingWindowService chargingWindowService)
    {
        _energyMixService = energyMixService;
        _chargingWindowService = chargingWindowService;
    }

    [HttpGet("mix")]
    public async Task<IActionResult> GetEnergyMix()
    {
        try
        {
            var result = await _energyMixService.GetThreeDayMixAsync();
            return Ok(result);
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, "External energy API is currently unavailable.");
        }
    }

    [HttpGet("chwindow")]
    public async Task<IActionResult> GetOptimalChargingWindow([FromQuery] int hours)
    {
        if (hours < 1 || hours > 6)
            return BadRequest("Hours must be between 1 and 6.");

        try
        {
            var result = await _chargingWindowService.GetOptimalWindowAsync(hours);
            return Ok(result);
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, "External energy API is currently unavailable.");
        }
    }
}
