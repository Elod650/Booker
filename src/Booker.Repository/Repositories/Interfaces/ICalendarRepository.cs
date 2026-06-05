namespace Booker.Repository.Repositories.Interfaces;

public interface ICalendarRepository
{
    Task AddCalendarAsync(Calendar newCalendar, CancellationToken cancellationToken = default);
    Task<List<int>> GetCalendarIdsAsync(
        Expression<Func<Calendar, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    );
    Task<List<Calendar>> GetCalendarsAsync(
        Expression<Func<Calendar, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    );
    Task<List<Calendar>> GetCalendarsForCustomerAsync(string customerId, CancellationToken cancellationToken = default);
}
