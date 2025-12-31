namespace Booker.ApiCaller.CallsForControllers;

public class ServiceApiCaller : IServiceApiCaller
{
    private readonly string _apiUrl;

    public ServiceApiCaller(IOptions<ApiCallerOptions> options)
    {
        _apiUrl = options.Value.ServiceApiUrl;
    }

    public async Task<List<ServiceDto>> GetServices(int calendarId)
    {
        string url = $"{_apiUrl}/{calendarId}";

        return await ApiCallerBase.SendAsync<List<ServiceDto>>(new ApiRequest(HttpMethod.Get, url));
    }

    public async Task<List<ServiceDto>> GetServices()
    {
        return await ApiCallerBase.SendAsync<List<ServiceDto>>(new ApiRequest(HttpMethod.Get, _apiUrl));
    }

    public async Task AddService(EditServiceRequest newService)
    {
        await ApiCallerBase.SendAsync<string>(new ApiRequest(HttpMethod.Post, _apiUrl, newService));
    }
}
