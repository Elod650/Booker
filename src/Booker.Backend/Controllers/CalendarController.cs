namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CalendarController(ICalendarService calendarService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<CalendarDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CalendarDto>>> GetCalendars(CancellationToken cancellationToken)
    {
        var calendars = await calendarService.GetCalendars(cancellationToken);
        return Ok(calendars);
    }

    [HttpPost, Authorize(Roles = "Admin, Provider")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddService(
        [FromBody] EditCalendarRequest newCalendar,
        CancellationToken cancellationToken
    )
    {
        var result = await calendarService.AddCalendar(newCalendar, cancellationToken);

        if (result is not null)
        {
            return BadRequest(result);
        }

        return Ok();
    }
}
