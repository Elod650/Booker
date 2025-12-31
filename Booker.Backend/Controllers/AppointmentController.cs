namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentController(IAppointmentRepository appointmentRepository) : ControllerBase
{
    [HttpGet("{calendarId:int}")]
    public async Task<ActionResult<List<AppointmentDto>>> GetAppointments(int calendarId)
    {
        var appointments = appointmentRepository.GetAppointments(calendarId);
        return Ok(appointments);
    }

    [HttpPost]
    public async Task<IActionResult> AddAppointment([FromBody] AppointmentDto newAppointment)
    {
        appointmentRepository.AddAppointment(newAppointment);
        return Ok();
    }
}
