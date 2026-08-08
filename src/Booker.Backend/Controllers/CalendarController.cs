namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CalendarController(ICalendarService calendarService, IValidatorService validatorService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<CalendarDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CalendarDto>>> GetCalendars(CancellationToken cancellationToken)
    {
        var calendars = await calendarService.GetCalendars(cancellationToken);
        return Ok(calendars);
    }

    [HttpGet]
    [Route("forOwner")]
    [ProducesResponseType(typeof(List<CalendarDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CalendarDto>>> GetCalendarsByOwnerId(CancellationToken cancellationToken)
    {
        var calendars = await calendarService.GetCalendarsByOwnerId(
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)!,
            cancellationToken
        );
        return Ok(calendars);
    }

    [HttpGet]
    [Route("forCustomer")]
    [ProducesResponseType(typeof(List<CalendarDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CalendarDto>>> GetCalendarsForCustomer(CancellationToken cancellationToken)
    {
        var calendars = await calendarService.GetCalendarsForCustomer(
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)!,
            cancellationToken
        );
        return Ok(calendars);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CalendarDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CalendarDto>> GetCalendarById(int id, CancellationToken cancellationToken)
    {
        var calendar = await calendarService.GetCalendarById(id, cancellationToken);

        if (calendar is null)
        {
            return NotFound();
        }

        return Ok(calendar);
    }

    [HttpDelete("{id:int}"), Authorize(Roles = "Admin, Provider")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCalendar(int id, CancellationToken cancellationToken)
    {
        var userIsOwner = await validatorService.ValidateCalendarOwnership(
            id,
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)!
        );

        if (!userIsOwner)
        {
            return Forbid("The user does not have permission to edit this calendar.");
        }

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

    [HttpPut, Authorize(Roles = "Admin, Provider")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCalendar(
        [FromBody] EditCalendarRequest updatedCalendar,
        CancellationToken cancellationToken
    )
    {
        if (updatedCalendar.Id is null)
        {
            return BadRequest("The Id must be specified when updating a calendar.");
        }

        var userIsOwner = await validatorService.ValidateCalendarOwnership(
            updatedCalendar.Id.Value,
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)!
        );

        if (!userIsOwner)
        {
            return Forbid("The user does not have permission to edit this calendar.");
        }

        var result = await calendarService.UpdateCalendar(updatedCalendar, cancellationToken);

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
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddCustomerToCalendar(
        [FromBody] AddCustomerToCalendarRequest request,
        CancellationToken cancellationToken
    )
    {
        var userIsOwner = await validatorService.ValidateCalendarOwnership(
            request.CalendarId,
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)!
        );

        if (!userIsOwner)
        {
            return Forbid("The user does not have permission to edit this calendar.");
        }

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
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<UserDto>>> GetCustomersForCalendar(
        [FromRoute] int calendarId,
        CancellationToken cancellationToken
    )
    {
        var userIsOwner = await validatorService.ValidateCalendarOwnership(
            calendarId,
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)!
        );

        if (!userIsOwner)
        {
            return Forbid("The user does not have permission to access this calendar.");
        }

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
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveCustomerFromCalendar(
        [FromBody] RemoveCustomerFromCalendarRequest request,
        CancellationToken cancellationToken
    )
    {
        var userIsOwner = await validatorService.ValidateCalendarOwnership(
            request.CalendarId,
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)!
        );

        if (!userIsOwner)
        {
            return Forbid("The user does not have permission to edit this calendar.");
        }

        var result = await calendarService.RemoveCustomerFromCalendar(request, cancellationToken);

        if (result is not null)
        {
            return BadRequest(result);
        }

        return Ok();
    }
}
