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
}
