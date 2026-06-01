namespace Booker.Repository.Repositories.Interfaces;

public interface ICalendarRepository
{
    Task AddCalendarAsync(Calendar newCalendar, CancellationToken cancellationToken = default);
    Task<List<Calendar>> GetCalendarsAsync(CancellationToken cancellationToken = default);
}
