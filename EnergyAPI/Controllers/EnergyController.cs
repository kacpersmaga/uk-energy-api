using EnergyAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnergyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnergyController : ControllerBase
{
    private readonly IEnergyMixService _energyMixService;

    public EnergyController(IEnergyMixService energyMixService)
    {
        _energyMixService = energyMixService;
    }

    [HttpGet("mix")]
    public async Task<IActionResult> GetEnergyMix()
    {
        var result = await _energyMixService.GetThreeDayMixAsync();
        return Ok(result);
    }
}
