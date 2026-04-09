namespace Booker.ApiCaller.CallsForControllers.Interfaces
{
    public interface IServiceApiCaller
    {
        Task AddService(EditServiceRequest newService, CancellationToken cancellationToken = default);
        Task DeleteServices(int id, CancellationToken cancellationToken = default);
        Task<ServiceDto> GetServiceById(int serviceId, CancellationToken cancellationToken = default);
        Task<List<ServiceDto>> GetServices(CancellationToken cancellationToken = default);
        Task<List<ServiceDto>> GetServicesForCalendar(int calendarId, CancellationToken cancellationToken = default);
        Task UpdateService(EditServiceRequest updatedService, CancellationToken cancellationToken = default);
    }
}
