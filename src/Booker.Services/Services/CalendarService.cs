namespace Booker.Services.Services;

public class CalendarService(ICalendarRepository calendarRepository, IMapper mapper) : ICalendarService
{
    public async Task<List<CalendarDto>> GetCalendars(CancellationToken cancellationToken = default)
    {
        return mapper.Map<List<CalendarDto>>(await calendarRepository.GetCalendarsAsync(cancellationToken));
    }
}
