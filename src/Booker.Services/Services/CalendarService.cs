namespace Booker.Services.Services;

public class CalendarService(ICalendarRepository calendarRepository) : ICalendarService
{
    public async Task<List<CalendarDto>> GetCalendars()
    {
        return calendarRepository.GetCalendars();
    }
}
