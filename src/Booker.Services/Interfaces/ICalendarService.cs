namespace Booker.Services.Interfaces;

public interface ICalendarService
{
    Task<string?> AddCalendar(
        EditCalendarRequest newCalendar,
        string userId,
        CancellationToken cancellationToken = default
    );
    Task<string?> AddCustomerToCalendar(
        AddCustomerToCalendarRequest request,
        CancellationToken cancellationToken = default
    );
    Task<string?> DeleteCalendar(int calendarId, CancellationToken cancellationToken = default);
    Task<List<CalendarDto>> GetCalendars(CancellationToken cancellationToken = default);
    Task<List<CalendarDto>> GetCalendarsByOwnerId(string ownerId, CancellationToken cancellationToken = default);
    Task<List<CalendarDto>> GetCalendarsForCustomer(string customerId, CancellationToken cancellationToken = default);
    Task<List<UserDto>?> GetCustomersForCalendar(int calendarId, CancellationToken cancellationToken = default);
    Task<string?> RemoveCustomerFromCalendar(
        RemoveCustomerFromCalendarRequest request,
        CancellationToken cancellationToken = default
    );
}
