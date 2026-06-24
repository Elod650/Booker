namespace Booker.ApiCaller.CallsForControllers.Interfaces;

public interface ICalendarApiCaller
{
    Task AddCalendar(EditCalendarRequest newCalendar, CancellationToken cancellationToken = default);
    Task<List<CalendarDto>> GetCalendarsByOwnerId(string ownerId, CancellationToken cancellationToken = default);
    Task<List<CalendarDto>> GetCalendars(CancellationToken cancellationToken = default);
    Task<List<CalendarDto>> GetCalendarsForCustomer(string customerId, CancellationToken cancellationToken = default);
    Task DeleteCalendar(int id, CancellationToken cancellationToken = default);
    Task<string?> AddCustomerToCalendar(
        AddCustomerToCalendarRequest request,
        CancellationToken cancellationToken = default
    );
    Task<List<UserDto>> GetCustomersForCalendar(int calendarId, CancellationToken cancellationToken = default);
    Task<string?> RemoveCustomerFromCalendarAsync(
        RemoveCustomerFromCalendarRequest request,
        CancellationToken cancellationToken = default
    );
}
