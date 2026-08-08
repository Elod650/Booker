namespace Booker.Services.Services;

public class CalendarService(
    ICalendarRepository calendarRepository,
    IAppointmentRepository appointmentRepository,
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

    public async Task<CalendarDto?> GetCalendarById(int calendarId, CancellationToken cancellationToken = default)
    {
        var calendar = await calendarRepository.GetCalendarByIdAsync(calendarId, cancellationToken: cancellationToken);

        if (calendar is null)
        {
            return null;
        }

        return mapper.Map<CalendarDto>(calendar);
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

        var workHoursError = ParseWorkHours(newCalendar, out _, out _);

        if (workHoursError is not null)
        {
            return workHoursError;
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

    public async Task<string?> UpdateCalendar(
        EditCalendarRequest updatedCalendar,
        CancellationToken cancellationToken = default
    )
    {
        if (updatedCalendar.Id is null)
        {
            return "The Id must be specified when updating a calendar.";
        }

        var calendarToUpdate = await calendarRepository.GetCalendarByIdAsync(
            updatedCalendar.Id.Value,
            asNoTracking: false,
            cancellationToken
        );

        if (calendarToUpdate is null)
        {
            return "There is no calendar with the provided Id.";
        }

        var workHoursError = ParseWorkHours(updatedCalendar, out var startTime, out var endTime);

        if (workHoursError is not null)
        {
            return workHoursError;
        }

        var conflictError = await ValidateNoAppointmentsOutsideWorkHours(
            calendarToUpdate.Id,
            startTime,
            endTime,
            cancellationToken
        );

        if (conflictError is not null)
        {
            return conflictError;
        }

        mapper.Map(updatedCalendar, calendarToUpdate);

        await calendarRepository.UpdateCalendarAsync(calendarToUpdate, cancellationToken);

        return null;
    }

    //The work hours are zone-less "HH:mm" strings, so they are always parsed with the invariant
    //culture rather than the ambient locale. See the timezone notes in CLAUDE.md.
    private static string? ParseWorkHours(EditCalendarRequest calendar, out TimeOnly startTime, out TimeOnly endTime)
    {
        startTime = default;
        endTime = default;

        if (
            !TimeOnly.TryParse(calendar.StartTime, CultureInfo.InvariantCulture, out startTime)
            || !TimeOnly.TryParse(calendar.EndTime, CultureInfo.InvariantCulture, out endTime)
        )
        {
            return "The format of the work hours is invalid. The correct format: HH:mm";
        }

        if (startTime >= endTime)
        {
            return "The start time must be earlier than the end time.";
        }

        return null;
    }

    //Narrowing the work hours must not silently strand bookings that already exist, so the edit is
    //rejected while any upcoming appointment falls outside the new window. Appointments are local
    //wall-clock values, hence DateTime.Now rather than DateTime.UtcNow.
    private async Task<string?> ValidateNoAppointmentsOutsideWorkHours(
        int calendarId,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken cancellationToken
    )
    {
        var appointments = await appointmentRepository.GetAppointmentsForCalendarAsync(
            calendarId,
            cancellationToken: cancellationToken
        );

        int conflictCount = appointments.Count(x =>
            x.StartTime >= DateTime.Now
            && (TimeOnly.FromDateTime(x.StartTime) < startTime || TimeOnly.FromDateTime(x.EndTime) > endTime)
        );

        if (conflictCount > 0)
        {
            return $"{conflictCount} upcoming appointment(s) fall outside the new work hours. "
                + "Move or cancel them before changing the hours.";
        }

        return null;
    }
}
