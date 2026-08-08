namespace Booker.ApiCaller.CallsForControllers.Interfaces;

public interface ICalendarApiCaller
{
    Task AddCalendar(EditCalendarRequest newCalendar, CancellationToken cancellationToken = default);
    Task UpdateCalendar(EditCalendarRequest updatedCalendar, CancellationToken cancellationToken = default);
    Task<CalendarDto> GetCalendarById(int calendarId, CancellationToken cancellationToken = default);
    Task<List<CalendarDto>> GetCalendarsByOwnerId(CancellationToken cancellationToken = default);
    Task<List<CalendarDto>> GetCalendars(CancellationToken cancellationToken = default);
    Task<List<CalendarDto>> GetCalendarsForCustomer(CancellationToken cancellationToken = default);
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
