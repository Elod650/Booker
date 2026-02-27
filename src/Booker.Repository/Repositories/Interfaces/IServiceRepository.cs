namespace Booker.Repository.Repositories.Interfaces
{
    public interface IServiceRepository
    {
        Task AddService(EditServiceRequest newService, CancellationToken cancellationToken = default);
        List<ServiceDto> GetServices();
        List<ServiceDto> GetServices(int calendarId);
    }
}
