namespace Booker.Services.Services;

public class ServiceService(IServiceRepository serviceRepository, IMapper mapper) : IServiceService
{
    public async Task<List<ServiceDto>> GetServices(CancellationToken cancellationToken = default)
    {
        return mapper.Map<List<ServiceDto>>(await serviceRepository.GetServicesAsync(cancellationToken));
    }

    public async Task<List<ServiceDto>> GetServices(int calendarId, CancellationToken cancellationToken = default)
    {
        return mapper.Map<List<ServiceDto>>(
            await serviceRepository.GetServicesForCalendarAsync(calendarId, cancellationToken)
        );
    }

    public async Task AddService(EditServiceRequest newService, CancellationToken cancellationToken = default)
    {
        await serviceRepository.AddService(mapper.Map<Service>(newService), cancellationToken);
    }
}
