namespace Booker.Services.Interfaces;

public interface ICalendarService
{
    Task<string?> AddCalendar(EditCalendarRequest newCalendar, CancellationToken cancellationToken = default);
    Task<List<CalendarDto>> GetCalendars(CancellationToken cancellationToken = default);
}
