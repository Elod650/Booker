namespace Booker.Repository.Repositories;

public class ServiceRepository(AppDbContext context) : IServiceRepository
{
    public async Task<List<Service>> GetServicesAsync(CancellationToken cancellationToken = default)
    {
        return await context.Services.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<List<Service>> GetServicesForCalendarAsync(
        int calendarId,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .Services.AsNoTracking()
            .Where(x => x.CalendarId == calendarId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddService(Service newService, CancellationToken cancellationToken = default)
    {
        await context.AddAsync(newService, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
