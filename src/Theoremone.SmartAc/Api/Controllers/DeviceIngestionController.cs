using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Theoremone.SmartAc.Api.Bindings;
using Theoremone.SmartAc.Api.Models;
using Theoremone.SmartAc.Api.Validations.Device;
using Theoremone.SmartAc.Application.DeviceWrapper;
using Theoremone.SmartAc.Application.DeviceWrapper.ModelsDTO;
using Theoremone.SmartAc.Domain.Entities;
using Theoremone.SmartAc.Infrastructure.Data.Extensions;
using Theoremone.SmartAc.Infrastructure.Identity;
using Theoremone.SmartAc.Infrastructure.Persistence;

namespace Theoremone.SmartAc.Api.Controllers;

[ApiController]
[Route("api/v1/device")]
[Authorize("DeviceIngestion")]
public class DeviceIngestionController : ControllerBase
{
    private readonly DeviceWrapper _deviceWrapper;
    private readonly SmartAcJwtService _smartAcJwtService;
    private readonly ILogger<DeviceIngestionController> _logger;

    public DeviceIngestionController(
        DeviceWrapper deviceWrapper,
        SmartAcJwtService smartAcJwtService,
        ILogger<DeviceIngestionController> logger)
    {
        _deviceWrapper = deviceWrapper;
        _smartAcJwtService = smartAcJwtService;
        _logger = logger;
    }

    /// <summary>
    /// Allow smart ac devices to register themselves  
    /// and get a jwt token for subsequent operations
    /// </summary>
    /// <param name="serialNumber">Unique device identifier burned into ROM</param>
    /// <param name="sharedSecret">Unique device shareble secret burned into ROM</param>
    /// <param name="firmwareVersion">Device firmware version at the moment of registering</param>
    /// <returns>A jwt token</returns>
    /// <response code="400">If any of the required data is not pressent or is invalid.</response>
    /// <response code="401">If something is wrong on the information provided.</response>
    /// <response code="200">If the registration has sucesfully generated a new jwt token.</response>
    [HttpPost("{serialNumber}/register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterDevice(
        [Required][FromRoute] string serialNumber,
        [Required][FromHeader(Name = "x-device-shared-secret")] string sharedSecret,
        [Required][SemanticVersion][FromQuery] string firmwareVersion)
    {
        // TODO: The firmware version should have semantic versioning format (RESOLVED)
        // a validation should be added and in case of failure it should return
        // the same response body returned when attributes annotated with [required] are not present
        // NOTE: Use out-of-the box functionalities here
        var device = await _deviceWrapper.GetRegisterDeviceDevice(new GetDeviceRegisterdQuery() { serialNumber = serialNumber, sharedSecret= sharedSecret });
        var (tokenId, jwtToken) = _smartAcJwtService.GenerateJwtFor(serialNumber, SmartAcJwtService.JwtScopeDeviceIngestionService);
        await _deviceWrapper.RegisterDevice(device, tokenId, firmwareVersion);
        return Ok(jwtToken);
    }

    /// <summary>
    /// Allow smart ac devices to send sensor readings in batch
    /// 
    /// This will additionally trigger analysis over the sensor readings
    /// to generate alerts based on it
    /// </summary>
    /// <param name="serialNumber">Unique device identifier burned into ROM.</param>
    /// <param name="sensorReadings">Collection of sensor readings send by a device.</param>
    /// <response code="401">If jwt token provided is invalid.</response>
    /// <response code="202">If sensor readings has sucesfully accepted.</response>
    /// <returns>No Content.</returns>
    [HttpPost("readings/batch")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> AddSensorReadings(
        [ModelBinder(BinderType = typeof(DeviceInfoBinder))] string serialNumber,
        [FromBody] IEnumerable<DeviceReadingRecord> sensorReadings)
    {
        var receivedDate = DateTime.UtcNow;
        var deviceReadings = sensorReadings.Select(reading => reading.ToDeviceReading(serialNumber, receivedDate)).ToList();
        await _deviceWrapper.AddDeviceReadings(deviceReadings, serialNumber);
        return Accepted();
    }
}
