namespace Booker.Repository.Repositories;

public class CalendarRepository : ICalendarRepository
{
    public List<CalendarDto> GetCalendars()
    {
        return Database.Calendars.Map();
    }
}
