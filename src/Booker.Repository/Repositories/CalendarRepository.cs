namespace Booker.Repository.Repositories;

public class CalendarRepository(AppDbContext context) : ICalendarRepository
{
    public async Task<List<Calendar>> GetCalendarsAsync(CancellationToken cancellationToken = default)
    {
        return await context.Calendars.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task AddCalendarAsync(Calendar newCalendar, CancellationToken cancellationToken = default)
    {
        await context.AddAsync(newCalendar, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
