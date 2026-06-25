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
    public async Task<IActionResult> AddCalendar(
        [FromBody] EditCalendarRequest newCalendar,
        CancellationToken cancellationToken
    )
    {
        var result = await calendarService.AddCalendar(
            newCalendar,
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)!,
            cancellationToken
        );

        if (result is not null)
        {
            return BadRequest(result);
        }

        return Ok();
    }

    [HttpPost, Authorize(Roles = "Admin, Provider")]
    [Route("addCustomer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddCustomerToCalendar(
        [FromBody] AddCustomerToCalendarRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await calendarService.AddCustomerToCalendar(request, cancellationToken);

        if (result is not null)
        {
            return BadRequest(result);
        }

        return Ok();
    }

    [HttpGet]
    [Route("{calendarId}/customers")]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserDto>>> GetCustomersForCalendar(
        [FromRoute] int calendarId,
        CancellationToken cancellationToken
    )
    {
        var users = await calendarService.GetCustomersForCalendar(calendarId, cancellationToken);

        if (users is null)
        {
            return BadRequest("Invalid calendar Id.");
        }

        return Ok(users);
    }

    [HttpPost, Authorize(Roles = "Admin, Provider")]
    [Route("removeCustomer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveCustomerFromCalendar(
        [FromBody] RemoveCustomerFromCalendarRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await calendarService.RemoveCustomerFromCalendar(request, cancellationToken);

        if (result is not null)
        {
            return BadRequest(result);
        }

        return Ok();
    }
}
