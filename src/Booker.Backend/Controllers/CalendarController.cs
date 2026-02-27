namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CalendarController(ICalendarRepository calendarRepository) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<CalendarDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CalendarDto>>> GetCalendars()
    {
        var calendars = calendarRepository.GetCalendars();
        return Ok(calendars);
    }
}
