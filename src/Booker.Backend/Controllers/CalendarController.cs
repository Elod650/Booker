namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CalendarController(ICalendarService calendarService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<CalendarDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CalendarDto>>> GetCalendars()
    {
        var calendars = await calendarService.GetCalendars();
        return Ok(calendars);
    }
}
