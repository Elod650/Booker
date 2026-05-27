namespace Booker.ApiCaller.CallsForControllers;

public class AppointmentApiCaller(IApiCallerBase apiCallerBase, IOptions<ApiCallerOptions> options)
    : IAppointmentApiCaller
{
    private readonly string _apiUrl = options.Value.AppointmentApiUrl;

    public async Task<List<AppointmentDto>> GetAppointments(
        int calendarId,
        CancellationToken cancellationToken = default
    )
    {
        string url = $"{_apiUrl}/{calendarId}";

        return await apiCallerBase.SendWithResponseAsync<List<AppointmentDto>>(
            ApiRequest.CreateGet(url),
            cancellationToken: cancellationToken
        );
    }

    public async Task AddAppointment(
        EditAppointmentRequest newAppointment,
        CancellationToken cancellationToken = default
    )
    {
        await apiCallerBase.SendAsync(
            ApiRequest.CreatePost(_apiUrl, newAppointment),
            cancellationToken: cancellationToken
        );
    }

    public async Task DeleteAppointment(int id, CancellationToken cancellationToken = default)
    {
        string url = $"{_apiUrl}/{id}";

        await apiCallerBase.SendWithResponseAsync(ApiRequest.CreateDelete(url), cancellationToken: cancellationToken);
    }
}
