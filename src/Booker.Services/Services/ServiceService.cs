namespace Booker.Services.Services;

public class ServiceService(
    IServiceRepository serviceRepository,
    ICalendarRepository calendarRepository,
    IMapper mapper
) : IServiceService
{
    public async Task<List<ServiceDto>> GetServicesForUser(string userId, CancellationToken cancellationToken = default)
    {
        var calendarsForUser = await calendarRepository.GetCalendarIdsAsync(
            x => x.OwnerId == userId,
            cancellationToken
        );

        if (calendarsForUser.Count == 0)
        {
            return [];
        }

        return mapper.Map<List<ServiceDto>>(
            await serviceRepository.GetServicesAsync(
                x => calendarsForUser.Contains(x.CalendarId),
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task<List<ServiceDto>> GetServicesForCalendar(
        int calendarId,
        CancellationToken cancellationToken = default
    )
    {
        return mapper.Map<List<ServiceDto>>(
            await serviceRepository.GetServicesAsync(
                x => x.CalendarId == calendarId,
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task<ServiceDto> GetServiceById(int serviceId, CancellationToken cancellationToken = default)
    {
        return mapper.Map<ServiceDto>(
            await serviceRepository.GetServiceByIdAsync(serviceId, cancellationToken: cancellationToken)
        );
    }

    public async Task<string?> AddService(EditServiceRequest newService, CancellationToken cancellationToken = default)
    {
        if (newService.Id is not null)
        {
            return "The Id has to be null when adding a new service.";
        }

        await serviceRepository.AddServiceAsync(mapper.Map<Service>(newService), cancellationToken);

        return null;
    }

    public async Task<string?> DeleteService(int serviceId, CancellationToken cancellationToken = default)
    {
        var serviceToDelete = await serviceRepository.GetServiceByIdAsync(
            serviceId,
            asNoTracking: false,
            cancellationToken
        );

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
        if (updatedService.Id is null)
        {
            return "The Id must be specified when updating a service.";
        }

        var serviceToUpdate = await serviceRepository.GetServiceByIdAsync(
            updatedService.Id.Value,
            asNoTracking: false,
            cancellationToken
        );
        if (serviceToUpdate is null)
        {
            return "There is no service with the provided Id.";
        }

        mapper.Map(updatedService, serviceToUpdate);

        await serviceRepository.UpdateServiceAsync(serviceToUpdate, cancellationToken);

        return null;
    }
}
