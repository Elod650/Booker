namespace Booker.Services.Services;

public class CalendarService(ICalendarRepository calendarRepository, IMapper mapper) : ICalendarService
{
    public async Task<List<CalendarDto>> GetCalendars()
    {
        return mapper.Map<List<CalendarDto>>(await calendarRepository.GetCalendarsAsync());
    }
}
