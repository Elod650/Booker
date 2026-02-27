namespace Booker.Services.Interfaces;

public interface ICalendarService
{
    Task<List<CalendarDto>> GetCalendars();
}
