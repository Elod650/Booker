namespace Booker.ApiCaller.CallsForControllers.Interfaces
{
    public interface ICalendarApiCaller
    {
        Task AddCalendar(EditCalendarRequest newCalendar, CancellationToken cancellationToken = default);
        Task<List<CalendarDto>> GetCalendars(int calendarId, CancellationToken cancellationToken = default);
        Task<List<CalendarDto>> GetCalendars(CancellationToken cancellationToken = default);
    }
}
