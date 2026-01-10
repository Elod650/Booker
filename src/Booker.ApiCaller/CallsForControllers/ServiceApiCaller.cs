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

        return await ApiCallerBase.SendWithResponseAsync<List<ServiceDto>>(ApiRequest.CreateGet(url));
    }

    public async Task<List<ServiceDto>> GetServices()
    {
        return await ApiCallerBase.SendWithResponseAsync<List<ServiceDto>>(ApiRequest.CreateGet(_apiUrl));
    }

    public async Task AddService(EditServiceRequest newService)
    {
        await ApiCallerBase.SendAsync(ApiRequest.CreatePost(_apiUrl, newService));
    }
}
