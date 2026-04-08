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

    public async Task<string?> DeleteService(int serviceId, CancellationToken cancellationToken = default)
    {
        var serviceToDelete = await serviceRepository.GetServiceByIdAsync(serviceId, cancellationToken);

        if (serviceToDelete is null)
        {
            return "There is no service with the provided Id.";
        }

        await serviceRepository.DeleteServiceAsync(serviceToDelete, cancellationToken);

        return null;
    }
}
