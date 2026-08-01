namespace Booker.Repository.Repositories;

public class ServiceRepository(AppDbContext context) : IServiceRepository
{
    public async Task<List<Service>> GetServicesAsync(
        Expression<Func<Service, bool>>? predicate = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        var query = asNoTracking ? context.Services.AsNoTracking() : context.Services;

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Service?> GetServiceByIdAsync(
        int id,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        var query = asNoTracking ? context.Services.AsNoTracking() : context.Services;

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddServiceAsync(Service newService, CancellationToken cancellationToken = default)
    {
        await context.AddAsync(newService, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteServiceAsync(Service serviceToDelete, CancellationToken cancellationToken = default)
    {
        await context.Entry(serviceToDelete).Collection(x => x.Appointments!).LoadAsync(cancellationToken);

        context.Services.Remove(serviceToDelete);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateServiceAsync(Service serviceToUpdate, CancellationToken cancellationToken = default)
    {
        context.Services.Update(serviceToUpdate);
        await context.SaveChangesAsync(cancellationToken);
    }
}
