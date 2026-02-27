namespace Booker.Services.Services;

public class ServiceService(IServiceRepository serviceRepository) : IServiceService
{
    public async Task<List<ServiceDto>> GetServices()
    {
        return serviceRepository.GetServices();
    }

    public async Task<List<ServiceDto>> GetServices(int calendarId)
    {
        return serviceRepository.GetServices(calendarId);
    }

    public async Task AddService(EditServiceRequest newService)
    {
        await serviceRepository.AddService(newService);
    }
}
