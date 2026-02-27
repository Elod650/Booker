namespace Booker.Services.Services;

public class ServiceService(IServiceRepository serviceRepository, IMapper mapper) : IServiceService
{
    public async Task<List<ServiceDto>> GetServices()
    {
        return mapper.Map<List<ServiceDto>>(await serviceRepository.GetServicesAsync());
    }

    public async Task<List<ServiceDto>> GetServices(int calendarId)
    {
        return mapper.Map<List<ServiceDto>>(await serviceRepository.GetServicesForCalendarAsync(calendarId));
    }

    public async Task AddService(EditServiceRequest newService)
    {
        await serviceRepository.AddService(mapper.Map<Service>(newService));
    }
}
