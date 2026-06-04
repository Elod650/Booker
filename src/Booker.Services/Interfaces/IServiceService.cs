namespace Booker.Services.Interfaces;

public interface IServiceService
{
    Task<string?> AddService(EditServiceRequest newService, CancellationToken cancellationToken = default);
    Task<string?> DeleteService(int serviceId, CancellationToken cancellationToken = default);
    Task<ServiceDto> GetServiceById(int serviceId, CancellationToken cancellationToken = default);
    Task<List<ServiceDto>> GetServicesForUser(string userId, CancellationToken cancellationToken = default);
    Task<List<ServiceDto>> GetServicesForCalendar(int calendarId, CancellationToken cancellationToken = default);
    Task<string?> UpdateService(EditServiceRequest updatedService, CancellationToken cancellationToken = default);
}
