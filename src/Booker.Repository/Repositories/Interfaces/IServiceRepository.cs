namespace Booker.Repository.Repositories.Interfaces
{
    public interface IServiceRepository
    {
        Task AddServiceAsync(Service newService, CancellationToken cancellationToken = default);
        Task DeleteServiceAsync(Service serviceToDelete, CancellationToken cancellationToken = default);
        Task<Service?> GetServiceByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Service>> GetServicesAsync(CancellationToken cancellationToken = default);
        Task<List<Service>> GetServicesForCalendarAsync(int calendarId, CancellationToken cancellationToken = default);
        Task UpdateServiceAsync(Service serviceToUpdate, CancellationToken cancellationToken = default);
    }
}
