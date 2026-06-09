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
        var result = await _energyMixService.GetThreeDayMixAsync();
        return Ok(result);
    }

    [HttpGet("chwindow")]
    public async Task<IActionResult> GetOptimalChargingWindow([FromQuery] int hours)
    {
        if (hours < 1 || hours > 6)
            return BadRequest("Hours must be between 1 and 6.");

        var result = await _chargingWindowService.GetOptimalWindowAsync(hours);
        return Ok(result);
    }
}
