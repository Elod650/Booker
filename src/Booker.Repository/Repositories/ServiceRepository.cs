namespace Booker.Repository.Repositories;

public class ServiceRepository(AppDbContext context) : IServiceRepository
{
    public async Task<List<Service>> GetServicesAsync(
        Expression<Func<Service, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Services.AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Service?> GetServiceByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddServiceAsync(Service newService, CancellationToken cancellationToken = default)
    {
        await context.AddAsync(newService, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteServiceAsync(Service serviceToDelete, CancellationToken cancellationToken = default)
    {
        context.Services.Remove(serviceToDelete);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateServiceAsync(Service serviceToUpdate, CancellationToken cancellationToken = default)
    {
        context.Services.Update(serviceToUpdate);
        await context.SaveChangesAsync(cancellationToken);
    }
}
