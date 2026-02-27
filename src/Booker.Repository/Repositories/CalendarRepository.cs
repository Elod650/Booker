namespace Booker.Repository.Repositories;

public class CalendarRepository(AppDbContext context) : ICalendarRepository
{
    public async Task<List<Calendar>> GetCalendarsAsync(CancellationToken cancellationToken = default)
    {
        return await context.Calendars.AsNoTracking().ToListAsync(cancellationToken);
    }
}
