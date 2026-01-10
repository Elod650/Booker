namespace Booker.ApiCaller.CallsForControllers.Interfaces
{
    public interface IServiceApiCaller
    {
        Task AddService(EditServiceRequest newService);
        Task<List<ServiceDto>> GetServices();
        Task<List<ServiceDto>> GetServices(int calendarId);
    }
}
