namespace Booker.Repository.Repositories.Interfaces;

public interface ICalendarRepository
{
    Task AddCalendarAsync(Calendar newCalendar, CancellationToken cancellationToken = default);
    Task<List<Calendar>> GetCalendarsAsync(
        Expression<Func<Calendar, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    );
}
