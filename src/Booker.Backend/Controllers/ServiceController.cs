namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServiceController(IServiceRepository serviceRepository) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServiceDto>>> GetServices()
    {
        var services = serviceRepository.GetServices();
        return Ok(services);
    }

    [HttpGet("{calendarId:int}")]
    [ProducesResponseType(typeof(List<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServiceDto>>> GetServices(int calendarId)
    {
        var services = serviceRepository.GetServices(calendarId);
        return Ok(services);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddService([FromBody] EditServiceRequest newService)
    {
        await serviceRepository.AddServices(newService);
        return Ok();
    }
}
