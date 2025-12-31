namespace Booker.Repository.Repositories;

public class CalendarRepository(AppDbContext context) : ICalendarRepository
{
    public List<CalendarDto> GetCalendars()
    {
        return context.Calendars.Map();
    }
}
