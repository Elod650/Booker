namespace Booker.Repository.Repositories.Interfaces;

public interface ICalendarRepository
{
    Task<List<Calendar>> GetCalendarsAsync(CancellationToken cancellationToken = default);
}
