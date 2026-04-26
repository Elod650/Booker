using Booker.Models.Enums;

namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ServiceController(IServiceService serviceService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServiceDto>>> GetServices(CancellationToken cancellationToken)
    {
        var services = await serviceService.GetServices(cancellationToken);
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
    public async Task<IActionResult> AddService(
        [FromBody] EditServiceRequest newService,
        CancellationToken cancellationToken
    )
    {
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
    public async Task<IActionResult> UpdateService(
        [FromBody] EditServiceRequest updatedService,
        CancellationToken cancellationToken
    )
    {
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
    public async Task<IActionResult> DeleteService(int id, CancellationToken cancellationToken)
    {
        var errorMessage = await serviceService.DeleteService(id, cancellationToken);

        if (errorMessage is not null)
        {
            return BadRequest(errorMessage);
        }

        return Ok();
    }
}
