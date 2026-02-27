namespace Booker.Services.Interfaces;

public interface IServiceService
{
    Task AddService(EditServiceRequest newService);
    Task<List<ServiceDto>> GetServices();
    Task<List<ServiceDto>> GetServices(int calendarId);
}
