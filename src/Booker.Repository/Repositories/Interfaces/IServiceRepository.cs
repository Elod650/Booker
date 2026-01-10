namespace Booker.Repository.Repositories.Interfaces
{
    public interface IServiceRepository
    {
        Task AddServices(EditServiceRequest newService);
        List<ServiceDto> GetServices();
        List<ServiceDto> GetServices(int calendarId);
    }
}
