namespace Booker.ApiCaller.CallsForControllers;

public class ServiceApiCaller : IServiceApiCaller
{
    private readonly string _apiUrl;

    public ServiceApiCaller(IOptions<ApiCallerOptions> options)
    {
        _apiUrl = options.Value.ServiceApiUrl;
    }

    public async Task<List<ServiceDto>> GetServices(int calendarId, CancellationToken cancellationToken = default)
    {
        string url = $"{_apiUrl}/{calendarId}";

        return await ApiCallerBase.SendWithResponseAsync<List<ServiceDto>>(
            ApiRequest.CreateGet(url),
            cancellationToken
        );
    }

    public async Task<List<ServiceDto>> GetServices(CancellationToken cancellationToken = default)
    {
        return await ApiCallerBase.SendWithResponseAsync<List<ServiceDto>>(
            ApiRequest.CreateGet(_apiUrl),
            cancellationToken
        );
    }

    public async Task AddService(EditServiceRequest newService, CancellationToken cancellationToken = default)
    {
        await ApiCallerBase.SendAsync(ApiRequest.CreatePost(_apiUrl, newService), cancellationToken);
    }

    public async Task DeleteServices(int id, CancellationToken cancellationToken = default)
    {
        string url = $"{_apiUrl}/{id}";

        await ApiCallerBase.SendAsync(ApiRequest.CreateDelete(url), cancellationToken);
    }
}
