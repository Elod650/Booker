namespace Booker.Services.Services;

public class CalendarService(ICalendarRepository calendarRepository, IMapper mapper) : ICalendarService
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
            await calendarRepository.GetCalendarsAsync(x => x.OwnerId == ownerId, cancellationToken)
        );
    }

    public async Task<List<CalendarDto>> GetCalendarsForCustomer(
        string customerId,
        CancellationToken cancellationToken = default
    )
    {
        return mapper.Map<List<CalendarDto>>(
            await calendarRepository.GetCalendarsForCustomerAsync(customerId, cancellationToken)
        );
    }

    public async Task<string?> AddCalendar(
        EditCalendarRequest newCalendar,
        CancellationToken cancellationToken = default
    )
    {
        if (newCalendar.Id != 0)
        {
            return "The Id has to be 0 when adding a new calendar.";
        }

        await calendarRepository.AddCalendarAsync(mapper.Map<Calendar>(newCalendar), cancellationToken);

        return null;
    }

    public async Task<string?> DeleteCalendar(int calendarId, CancellationToken cancellationToken = default)
    {
        var calendarToDelete = await calendarRepository.GetCalendarByIdAsync(calendarId, cancellationToken);

        if (calendarToDelete is null)
        {
            return "There is no calendar with the provided Id.";
        }

        await calendarRepository.DeleteCalendarAsync(calendarToDelete, cancellationToken);

        return null;
    }
}
