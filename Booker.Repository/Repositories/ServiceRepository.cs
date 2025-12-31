namespace Booker.Repository.Repositories;

public class ServiceRepository : IServiceRepository
{
    public List<ServiceDto> GetServices()
    {
        return Database.Services.Map();
    }

    public List<ServiceDto> GetServices(int calendarId)
    {
        return Database.Services.Where(x => x.CalendarId == calendarId).Map();
    }
}
