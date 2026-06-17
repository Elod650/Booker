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

    [HttpGet]
    [Route("forOwner/{ownerId}")]
    [ProducesResponseType(typeof(List<CalendarDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CalendarDto>>> GetCalendarsByOwnerId(
        [FromRoute] string ownerId,
        CancellationToken cancellationToken
    )
    {
        var calendars = await calendarService.GetCalendarsByOwnerId(ownerId, cancellationToken);
        return Ok(calendars);
    }

    [HttpGet]
    [Route("forCustomer/{customerId}")]
    [ProducesResponseType(typeof(List<CalendarDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CalendarDto>>> GetCalendarsForCustomer(
        [FromRoute] string customerId,
        CancellationToken cancellationToken
    )
    {
        var calendars = await calendarService.GetCalendarsForCustomer(customerId, cancellationToken);
        return Ok(calendars);
    }

    [HttpDelete("{id:int}"), Authorize(Roles = "Admin, Provider")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteService(int id, CancellationToken cancellationToken)
    {
        var errorMessage = await calendarService.DeleteCalendar(id, cancellationToken);

        if (errorMessage is not null)
        {
            return BadRequest(errorMessage);
        }

        return Ok();
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
