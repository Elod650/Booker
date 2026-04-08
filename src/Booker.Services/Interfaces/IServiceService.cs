namespace Booker.Services.Interfaces;

public interface IServiceService
{
    Task AddService(EditServiceRequest newService, CancellationToken cancellationToken = default);
    Task<string?> DeleteService(int serviceId, CancellationToken cancellationToken = default);
    Task<List<ServiceDto>> GetServices(CancellationToken cancellationToken = default);
    Task<List<ServiceDto>> GetServices(int calendarId, CancellationToken cancellationToken = default);
}
