namespace Booker.Repository.Repositories.Interfaces
{
    public interface IServiceRepository
    {
        List<ServiceDto> GetServices();
        List<ServiceDto> GetServices(int calendarId);
    }
}
