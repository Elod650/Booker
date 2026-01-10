namespace Booker.Repository.Repositories;

public class ServiceRepository(AppDbContext context) : IServiceRepository
{
    public List<ServiceDto> GetServices()
    {
        return context.Services.Map();
    }

    public List<ServiceDto> GetServices(int calendarId)
    {
        return context.Services.Where(x => x.CalendarId == calendarId).Map();
    }

    public void AddServices(EditServiceRequest newService)
    {
        context.Add(newService.Map());
        context.SaveChanges();
    }
}
