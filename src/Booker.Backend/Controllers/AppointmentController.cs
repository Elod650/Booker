namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AppointmentController(IAppointmentService appointmentService, IValidatorService validatorService)
    : ControllerBase
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
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAppointment(
        [FromBody] EditAppointmentRequest newAppointment,
        CancellationToken cancellationToken
    )
    {
        var userHasAccess = await validatorService.ValidateCalendarAccess(
            newAppointment.CalendarId,
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)!
        );

        if (!userHasAccess)
        {
            return Forbid("The user does not have permission to add appointment to this calendar.");
        }

        var errorMessage = await appointmentService.AddAppointment(
            newAppointment,
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)!,
            cancellationToken
        );

        if (errorMessage is not null)
        {
            return BadRequest(errorMessage);
        }

        return Ok();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAppointment(int id, CancellationToken cancellationToken)
    {
        var userIsOwner = await validatorService.ValidateAppointmentOwnership(
            id,
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)!
        );

        if (!userIsOwner)
        {
            return Forbid("The user does not have permission to delete this appointment.");
        }

        var errorMessage = await appointmentService.DeleteAppointment(id, cancellationToken);

        if (errorMessage is not null)
        {
            return BadRequest(errorMessage);
        }

        return Ok();
    }
}
