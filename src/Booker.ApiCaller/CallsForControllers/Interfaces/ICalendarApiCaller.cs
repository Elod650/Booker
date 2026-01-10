namespace Booker.ApiCaller.CallsForControllers.Interfaces
{
    public interface ICalendarApiCaller
    {
        Task<List<CalendarDto>> GetCalendars(int calendarId, CancellationToken cancellationToken = default);
        Task<List<CalendarDto>> GetCalendars(CancellationToken cancellationToken = default);
    }
}
