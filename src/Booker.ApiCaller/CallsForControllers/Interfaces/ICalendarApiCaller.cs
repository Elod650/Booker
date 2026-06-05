namespace Booker.ApiCaller.CallsForControllers.Interfaces;

public interface ICalendarApiCaller
{
    Task AddCalendar(EditCalendarRequest newCalendar, CancellationToken cancellationToken = default);
    Task<List<CalendarDto>> GetCalendarsByOwnerId(string ownerId, CancellationToken cancellationToken = default);
    Task<List<CalendarDto>> GetCalendars(CancellationToken cancellationToken = default);
    Task<List<CalendarDto>> GetCalendarsForCustomer(string customerId, CancellationToken cancellationToken = default);
}
