namespace Booker.Repository.Repositories;

public class ServiceRepository(AppDbContext context, IMapper mapper) : IServiceRepository
{
    public List<ServiceDto> GetServices()
    {
        return mapper.Map<List<ServiceDto>>(context.Services);
    }

    public List<ServiceDto> GetServices(int calendarId)
    {
        return mapper.Map<List<ServiceDto>>(context.Services.Where(x => x.CalendarId == calendarId));
    }

    public async Task AddServices(EditServiceRequest newService, CancellationToken cancellationToken = default)
    {
        await context.AddAsync(mapper.Map<Service>(newService), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
