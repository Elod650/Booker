namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentController(IAppointmentService appointmentService) : ControllerBase
{
    [HttpGet("{calendarId:int}")]
    [ProducesResponseType(typeof(List<AppointmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AppointmentDto>>> GetAppointments(
        int calendarId,
        CancellationToken cancellationToken
    )
    {
        var appointments = await appointmentService.GetAppointments(calendarId, cancellationToken);
        return Ok(appointments);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddAppointment(
        [FromBody] AppointmentDto newAppointment,
        CancellationToken cancellationToken
    )
    {
        await appointmentService.AddAppointment(newAppointment, cancellationToken);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAppointment(int id, CancellationToken cancellationToken)
    {
        var errorMessage = await appointmentService.DeleteAppointment(id, cancellationToken);

        if (errorMessage is not null)
        {
            return BadRequest(errorMessage);
        }

        return Ok();
    }
}
