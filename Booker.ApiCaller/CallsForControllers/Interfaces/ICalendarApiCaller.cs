namespace Booker.ApiCaller.CallsForControllers.Interfaces
{
    public interface ICalendarApiCaller
    {
        Task<List<CalendarDto>> GetCalendars(int calendarId);
        Task<List<CalendarDto>> GetCalendars();
    }
}
