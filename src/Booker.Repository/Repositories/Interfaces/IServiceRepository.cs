namespace Booker.Repository.Repositories.Interfaces
{
    public interface IServiceRepository
    {
        Task AddServices(EditServiceRequest newService, CancellationToken cancellationToken = default);
        List<ServiceDto> GetServices();
        List<ServiceDto> GetServices(int calendarId);
    }
}
