using System.Text.Json.Serialization;
using Dotnet.Serilog.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Dotnet.Serilog.API.Controllers;

[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
{
    private readonly ILogger<LogController> _logger;

    public LogController(ILogger<LogController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get([FromQuery] string name)
    {
        try
        {
            _logger.LogInformation($"Call Endpoint GET:/log with paramater {name}");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
            return BadRequest();
        }
    }

    [HttpPost]
    public IActionResult Post([FromBody] LogDto dto)
    {
        try
        {
            _logger.LogInformation($"Call Endpoint POST:/log with body {JsonSerializer.Serialize(dto)}");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
            return BadRequest();
        }
    }
}