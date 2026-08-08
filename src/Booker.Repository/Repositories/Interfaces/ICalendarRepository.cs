namespace Booker.Repository.Repositories.Interfaces;

public interface ICalendarRepository
{
    Task AddCalendarAsync(Calendar newCalendar, CancellationToken cancellationToken = default);
    Task AddCustomerToCalendarAsync(CalendarsXCustomers toAdd, CancellationToken cancellationToken = default);
    Task DeleteCalendarAsync(Calendar calendarToDelete, CancellationToken cancellationToken = default);
    Task<Calendar?> GetCalendarByIdAsync(
        int id,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<List<int>> GetCalendarIdsAsync(
        Expression<Func<Calendar, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    );
    Task<List<Calendar>> GetCalendarsAsync(
        Expression<Func<Calendar, bool>>? predicate = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<List<Calendar>> GetCalendarsForCustomerAsync(
        string customerId,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<List<ApplicationUser?>> GetCustomersForCalendarAsync(
        int calendarId,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<bool> IsCustomerOnCalendarAsync(
        int calendarId,
        string customerId,
        CancellationToken cancellationToken = default
    );
    Task RemoveCustomerFromCalendarAsync(string userId, int calendarId, CancellationToken cancellationToken = default);
    Task UpdateCalendarAsync(Calendar calendarToUpdate, CancellationToken cancellationToken = default);
}
