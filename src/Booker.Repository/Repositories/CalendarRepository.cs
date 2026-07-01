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

    public async Task<List<Calendar>> GetCalendarsForCustomerAsync(
        string customerId,
        CancellationToken cancellationToken = default
    )
    {
        var calendarIdsForCustomer = context
            .CalendarsXCustomers.AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .Select(x => x.CalendarId);

        var query = context.Calendars.AsNoTracking().Where(x => calendarIdsForCustomer.Contains(x.Id));

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

    public async Task<Calendar?> GetCalendarByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Calendars.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task DeleteCalendarAsync(Calendar calendarToDelete, CancellationToken cancellationToken = default)
    {
        context.Calendars.Remove(calendarToDelete);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddCustomerToCalendarAsync(
        CalendarsXCustomers toAdd,
        CancellationToken cancellationToken = default
    )
    {
        await context.CalendarsXCustomers.AddAsync(toAdd, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ApplicationUser?>> GetCustomersForCalendarAsync(
        int calendarId,
        CancellationToken cancellationToken = default
    )
    {
        var query = context
            .CalendarsXCustomers.AsNoTracking()
            .Where(x => x.CalendarId == calendarId)
            .Include(x => x.Customer)
            .Select(x => x.Customer);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task RemoveCustomerFromCalendarAsync(
        string userId,
        int calendarId,
        CancellationToken cancellationToken = default
    )
    {
        var connection = await context.CalendarsXCustomers.FirstOrDefaultAsync(x =>
            x.CustomerId == userId && x.CalendarId == calendarId
        );

        if (connection is null)
        {
            return;
        }

        context.CalendarsXCustomers.Remove(connection);
        await context.SaveChangesAsync(cancellationToken);
    }
}
