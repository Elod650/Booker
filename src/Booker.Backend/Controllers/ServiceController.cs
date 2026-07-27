namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ServiceController(IServiceService serviceService, IValidatorService validatorService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServiceDto>>> GetServicesForUser(CancellationToken cancellationToken)
    {
        var services = await serviceService.GetServicesForUser(
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)!,
            cancellationToken
        );
        return Ok(services);
    }

    [HttpGet("calendar/{calendarId:int}")]
    [ProducesResponseType(typeof(List<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServiceDto>>> GetServicesForCalendar(
        int calendarId,
        CancellationToken cancellationToken
    )
    {
        var services = await serviceService.GetServicesForCalendar(calendarId, cancellationToken);
        return Ok(services);
    }

    [HttpGet("{serviceId:int}")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ServiceDto>> GetServiceById(int serviceId, CancellationToken cancellationToken)
    {
        var services = await serviceService.GetServiceById(serviceId, cancellationToken);
        return Ok(services);
    }

    [HttpPost, Authorize(Roles = "Admin, Provider")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddService(
        [FromBody] EditServiceRequest newService,
        CancellationToken cancellationToken
    )
    {
        var userIsOwner = await validatorService.ValidateCalendarOwnership(
            newService.CalendarId,
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)!
        );

        if (!userIsOwner)
        {
            return Forbid("The user does not have permission to edit this calendar.");
        }

        var result = await serviceService.AddService(newService, cancellationToken);

        if (result is not null)
        {
            return BadRequest(result);
        }

        return Ok();
    }

    [HttpPut, Authorize(Roles = "Admin, Provider")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateService(
        [FromBody] EditServiceRequest updatedService,
        CancellationToken cancellationToken
    )
    {
        var userIsOwner = await validatorService.ValidateCalendarOwnership(
            updatedService.CalendarId,
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)!
        );

        if (!userIsOwner)
        {
            return Forbid("The user does not have permission to edit this calendar.");
        }

        var result = await serviceService.UpdateService(updatedService, cancellationToken);

        if (result is not null)
        {
            return BadRequest(result);
        }

        return Ok();
    }

    [HttpDelete("{id:int}"), Authorize(Roles = "Admin, Provider")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteService(int id, CancellationToken cancellationToken)
    {
        var userIsOwner = await validatorService.ValidateCalendarOwnership(
            id,
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)!
        );

        if (!userIsOwner)
        {
            return Forbid("The user does not have permission to edit this calendar.");
        }

        var errorMessage = await serviceService.DeleteService(id, cancellationToken);

        if (errorMessage is not null)
        {
            return BadRequest(errorMessage);
        }

        return Ok();
    }
}
