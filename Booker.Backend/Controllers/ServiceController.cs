namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServiceController(IServiceRepository serviceRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ServiceDto>>> GetServices()
    {
        var services = serviceRepository.GetServices();
        return Ok(services);
    }

    [HttpGet("{calendarId:int}")]
    public async Task<ActionResult<List<ServiceDto>>> GetServices(int calendarId)
    {
        var services = serviceRepository.GetServices(calendarId);
        return Ok(services);
    }

    [HttpPost]
    public async Task<IActionResult> AddService([FromBody] EditServiceRequest newService)
    {
        serviceRepository.AddServices(newService);
        return Ok();
    }
}
