namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentController(IAppointmentRepository appointmentRepository) : ControllerBase
{
    [HttpGet("{calendarId:int}")]
    [ProducesResponseType(typeof(List<AppointmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AppointmentDto>>> GetAppointments(int calendarId)
    {
        var appointments = appointmentRepository.GetAppointments(calendarId);
        return Ok(appointments);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddAppointment([FromBody] AppointmentDto newAppointment)
    {
        await appointmentRepository.AddAppointment(newAppointment);
        return Ok();
    }
}
