namespace Booker.Services.Services;

public class ServiceService(IServiceRepository serviceRepository, IMapper mapper) : IServiceService
{
    public async Task<List<ServiceDto>> GetServices(CancellationToken cancellationToken = default)
    {
        return mapper.Map<List<ServiceDto>>(await serviceRepository.GetServicesAsync(cancellationToken));
    }

    public async Task<List<ServiceDto>> GetServicesForCalendar(
        int calendarId,
        CancellationToken cancellationToken = default
    )
    {
        return mapper.Map<List<ServiceDto>>(
            await serviceRepository.GetServicesForCalendarAsync(calendarId, cancellationToken)
        );
    }

    public async Task<ServiceDto> GetServiceById(int serviceId, CancellationToken cancellationToken = default)
    {
        return mapper.Map<ServiceDto>(await serviceRepository.GetServiceByIdAsync(serviceId, cancellationToken));
    }

    public async Task<string?> AddService(EditServiceRequest newService, CancellationToken cancellationToken = default)
    {
        if (newService.Id != 0)
        {
            return "The Id has to be 0 when adding a new service.";
        }

        await serviceRepository.AddServiceAsync(mapper.Map<Service>(newService), cancellationToken);

        return null;
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

    public async Task<string?> UpdateService(
        EditServiceRequest updatedService,
        CancellationToken cancellationToken = default
    )
    {
        var serviceToUpdate = await serviceRepository.GetServiceByIdAsync(updatedService.Id, cancellationToken);
        if (serviceToUpdate is null)
        {
            return "There is no service with the provided Id.";
        }

        serviceToUpdate = mapper.Map<Service>(updatedService);

        await serviceRepository.UpdateServiceAsync(serviceToUpdate, cancellationToken);

        return null;
    }
}
