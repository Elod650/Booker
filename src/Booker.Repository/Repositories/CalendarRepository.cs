namespace Booker.Repository.Repositories;

public class CalendarRepository(AppDbContext context) : ICalendarRepository
{
    public async Task<List<Calendar>> GetCalendarsAsync(
        Expression<Func<Calendar, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Calendars.AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<int>> GetCalendarIdsAsync(
        Expression<Func<Calendar, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Calendars.AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.Select(x => x.Id).ToListAsync(cancellationToken);
    }

    public async Task AddCalendarAsync(Calendar newCalendar, CancellationToken cancellationToken = default)
    {
        await context.AddAsync(newCalendar, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
