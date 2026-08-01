namespace Booker.Repository.Repositories.Interfaces;

public interface IServiceRepository
{
    Task AddServiceAsync(Service newService, CancellationToken cancellationToken = default);
    Task DeleteServiceAsync(Service serviceToDelete, CancellationToken cancellationToken = default);
    Task<Service?> GetServiceByIdAsync(int id, bool asNoTracking = true, CancellationToken cancellationToken = default);
    Task<List<Service>> GetServicesAsync(
        Expression<Func<Service, bool>>? predicate = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    );
    Task UpdateServiceAsync(Service serviceToUpdate, CancellationToken cancellationToken = default);
}
