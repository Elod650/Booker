namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentController(IAppointmentService appointmentService) : ControllerBase
{
    [HttpGet("{calendarId:int}")]
    [ProducesResponseType(typeof(List<AppointmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AppointmentDto>>> GetAppointments(int calendarId)
    {
        var appointments = await appointmentService.GetAppointments(calendarId);
        return Ok(appointments);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddAppointment([FromBody] AppointmentDto newAppointment)
    {
        await appointmentService.AddAppointment(newAppointment);
        return Ok();
    }
}
