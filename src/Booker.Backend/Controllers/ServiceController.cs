namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServiceController(IServiceService serviceService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServiceDto>>> GetServices(CancellationToken cancellationToken)
    {
        var services = await serviceService.GetServices(cancellationToken);
        return Ok(services);
    }

    [HttpGet("{calendarId:int}")]
    [ProducesResponseType(typeof(List<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServiceDto>>> GetServices(int calendarId, CancellationToken cancellationToken)
    {
        var services = await serviceService.GetServices(calendarId, cancellationToken);
        return Ok(services);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddService(
        [FromBody] EditServiceRequest newService,
        CancellationToken cancellationToken
    )
    {
        await serviceService.AddService(newService, cancellationToken);
        return Ok();
    }
}
