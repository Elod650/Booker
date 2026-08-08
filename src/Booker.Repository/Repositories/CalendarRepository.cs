namespace Booker.Repository.Repositories;

public class CalendarRepository(AppDbContext context) : ICalendarRepository
{
    public async Task<List<Calendar>> GetCalendarsAsync(
        Expression<Func<Calendar, bool>>? predicate = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        var query = asNoTracking ? context.Calendars.AsNoTracking() : context.Calendars;

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<Calendar>> GetCalendarsForCustomerAsync(
        string customerId,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        var calendarIdsForCustomer = context
            .CalendarsXCustomers.AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .Select(x => x.CalendarId);

        var query = asNoTracking ? context.Calendars.AsNoTracking() : context.Calendars;

        return await query.Where(x => calendarIdsForCustomer.Contains(x.Id)).ToListAsync(cancellationToken);
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

    public async Task<Calendar?> GetCalendarByIdAsync(
        int id,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        var query = asNoTracking ? context.Calendars.AsNoTracking() : context.Calendars;

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task DeleteCalendarAsync(Calendar calendarToDelete, CancellationToken cancellationToken = default)
    {
        var entry = context.Entry(calendarToDelete);

        await entry.Collection(x => x.Appointments!).LoadAsync(cancellationToken);
        await entry.Collection(x => x.Services!).LoadAsync(cancellationToken);
        await entry.Collection(x => x.CalendarsXCustomers!).LoadAsync(cancellationToken);

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
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        var query = asNoTracking ? context.CalendarsXCustomers.AsNoTracking() : context.CalendarsXCustomers;

        return await query
            .Where(x => x.CalendarId == calendarId)
            .Include(x => x.Customer)
            .Select(x => x.Customer)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsCustomerOnCalendarAsync(
        int calendarId,
        string customerId,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .CalendarsXCustomers.AsNoTracking()
            .AnyAsync(x => x.CalendarId == calendarId && x.CustomerId == customerId, cancellationToken);
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

        //Not a cascade but a business rule: the customer loses access to the calendar, so their
        //upcoming bookings on it are dropped. Appointments that already started are kept as history.
        var upcomingAppointments = await context
            .Appointments.Where(x => x.CalendarId == calendarId && x.UserId == userId && x.StartTime >= DateTime.Now)
            .ToListAsync(cancellationToken);

        context.Appointments.RemoveRange(upcomingAppointments);
        context.CalendarsXCustomers.Remove(connection);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCalendarAsync(Calendar calendarToUpdate, CancellationToken cancellationToken = default)
    {
        context.Calendars.Update(calendarToUpdate);

        await context.SaveChangesAsync(cancellationToken);
    }
}
