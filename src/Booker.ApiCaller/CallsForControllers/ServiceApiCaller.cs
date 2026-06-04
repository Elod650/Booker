namespace Booker.ApiCaller.CallsForControllers;

public class ServiceApiCaller(IApiCallerBase apiCallerBase, IOptions<ApiCallerOptions> options) : IServiceApiCaller
{
    private readonly string _apiUrl = options.Value.ServiceApiUrl;

    public async Task<List<ServiceDto>> GetServicesForCalendar(
        int calendarId,
        CancellationToken cancellationToken = default
    )
    {
        string url = $"{_apiUrl}/calendar/{calendarId}";

        return await apiCallerBase.SendWithResponseAsync<List<ServiceDto>>(
            ApiRequest.CreateGet(url),
            cancellationToken: cancellationToken
        );
    }

    public async Task<List<ServiceDto>> GetServicesForUser(string userId, CancellationToken cancellationToken = default)
    {
        string url = $"{_apiUrl}/{userId}";
        return await apiCallerBase.SendWithResponseAsync<List<ServiceDto>>(
            ApiRequest.CreateGet(url),
            cancellationToken: cancellationToken
        );
    }

    public async Task<ServiceDto> GetServiceById(int serviceId, CancellationToken cancellationToken = default)
    {
        string url = $"{_apiUrl}/{serviceId}";

        return await apiCallerBase.SendWithResponseAsync<ServiceDto>(
            ApiRequest.CreateGet(url),
            cancellationToken: cancellationToken
        );
    }

    public async Task AddService(EditServiceRequest newService, CancellationToken cancellationToken = default)
    {
        await apiCallerBase.SendAsync(ApiRequest.CreatePost(_apiUrl, newService), cancellationToken: cancellationToken);
    }

    public async Task UpdateService(EditServiceRequest updatedService, CancellationToken cancellationToken = default)
    {
        await apiCallerBase.SendAsync(
            ApiRequest.CreatePut(_apiUrl, updatedService),
            cancellationToken: cancellationToken
        );
    }

    public async Task DeleteServices(int id, CancellationToken cancellationToken = default)
    {
        string url = $"{_apiUrl}/{id}";

        await apiCallerBase.SendAsync(ApiRequest.CreateDelete(url), cancellationToken: cancellationToken);
    }
}
