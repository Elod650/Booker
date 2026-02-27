namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServiceController(IServiceService serviceService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServiceDto>>> GetServices()
    {
        var services = await serviceService.GetServices();
        return Ok(services);
    }

    [HttpGet("{calendarId:int}")]
    [ProducesResponseType(typeof(List<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServiceDto>>> GetServices(int calendarId)
    {
        var services = await serviceService.GetServices(calendarId);
        return Ok(services);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddService([FromBody] EditServiceRequest newService)
    {
        await serviceService.AddService(newService);
        return Ok();
    }
}
