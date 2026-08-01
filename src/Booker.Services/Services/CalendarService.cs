namespace Booker.Services.Services;

public class CalendarService(
    ICalendarRepository calendarRepository,
    UserManager<ApplicationUser> userManager,
    IMapper mapper
) : ICalendarService
{
    public async Task<List<CalendarDto>> GetCalendars(CancellationToken cancellationToken = default)
    {
        return mapper.Map<List<CalendarDto>>(
            await calendarRepository.GetCalendarsAsync(cancellationToken: cancellationToken)
        );
    }

    public async Task<List<CalendarDto>> GetCalendarsByOwnerId(
        string ownerId,
        CancellationToken cancellationToken = default
    )
    {
        return mapper.Map<List<CalendarDto>>(
            await calendarRepository.GetCalendarsAsync(x => x.OwnerId == ownerId, cancellationToken: cancellationToken)
        );
    }

    public async Task<List<CalendarDto>> GetCalendarsForCustomer(
        string customerId,
        CancellationToken cancellationToken = default
    )
    {
        return mapper.Map<List<CalendarDto>>(
            await calendarRepository.GetCalendarsForCustomerAsync(customerId, cancellationToken: cancellationToken)
        );
    }

    public async Task<string?> AddCalendar(
        EditCalendarRequest newCalendar,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        if (newCalendar.Id is not null)
        {
            return "The Id has to be null when adding a new calendar.";
        }

        var newCalendarEntity = new Calendar { OwnerId = userId };
        mapper.Map(newCalendar, newCalendarEntity);

        await calendarRepository.AddCalendarAsync(newCalendarEntity, cancellationToken);

        return null;
    }

    public async Task<string?> DeleteCalendar(int calendarId, CancellationToken cancellationToken = default)
    {
        var calendarToDelete = await calendarRepository.GetCalendarByIdAsync(
            calendarId,
            asNoTracking: false,
            cancellationToken
        );

        if (calendarToDelete is null)
        {
            return "There is no calendar with the provided Id.";
        }

        await calendarRepository.DeleteCalendarAsync(calendarToDelete, cancellationToken);

        return null;
    }

    public async Task<string?> AddCustomerToCalendar(
        AddCustomerToCalendarRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userManager.FindByEmailAsync(request.CustomerEmail);

        if (user is null)
        {
            return "There is no user with this email.";
        }

        var calendar = await calendarRepository.GetCalendarByIdAsync(
            request.CalendarId,
            cancellationToken: cancellationToken
        );

        if (calendar is null)
        {
            return "There is no calendar with the provided Id.";
        }

        var connections = await calendarRepository.GetCustomersForCalendarAsync(
            calendar.Id,
            cancellationToken: cancellationToken
        );

        if (connections.Any(x => x.Id == user.Id))
        {
            return "The user is already added to the clanedar.";
        }

        var toAdd = new CalendarsXCustomers { CalendarId = calendar.Id, CustomerId = user.Id };

        await calendarRepository.AddCustomerToCalendarAsync(toAdd, cancellationToken);

        return null;
    }

    public async Task<List<UserDto>?> GetCustomersForCalendar(
        int calendarId,
        CancellationToken cancellationToken = default
    )
    {
        var calendar = await calendarRepository.GetCalendarByIdAsync(calendarId, cancellationToken: cancellationToken);

        if (calendar is null)
        {
            return null;
        }

        return mapper.Map<List<UserDto>>(
            await calendarRepository.GetCustomersForCalendarAsync(calendar.Id, cancellationToken: cancellationToken)
        );
    }

    public async Task<string?> RemoveCustomerFromCalendar(
        RemoveCustomerFromCalendarRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userManager.FindByEmailAsync(request.CustomerEmail);

        if (user is null)
        {
            return "There is no user with this email.";
        }

        var calendar = await calendarRepository.GetCalendarByIdAsync(
            request.CalendarId,
            cancellationToken: cancellationToken
        );

        if (calendar is null)
        {
            return "There is no calendar with the provided Id.";
        }

        await calendarRepository.RemoveCustomerFromCalendarAsync(user.Id, calendar.Id, cancellationToken);

        return null;
    }
}
