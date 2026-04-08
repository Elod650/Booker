namespace Booker.ApiCaller.CallsForControllers;

public class AppointmentApiCaller : IAppointmentApiCaller
{
    private readonly string _apiUrl;

    public AppointmentApiCaller(IOptions<ApiCallerOptions> options)
    {
        _apiUrl = options.Value.AppointmentApiUrl;
    }

    public async Task<List<AppointmentDto>> GetAppointments(
        int calendarId,
        CancellationToken cancellationToken = default
    )
    {
        string url = $"{_apiUrl}/{calendarId}";

        return await ApiCallerBase.SendWithResponseAsync<List<AppointmentDto>>(
            ApiRequest.CreateGet(url),
            cancellationToken
        );
    }

    public async Task AddAppointment(AppointmentDto newAppointment, CancellationToken cancellationToken = default)
    {
        await ApiCallerBase.SendAsync(ApiRequest.CreatePost(_apiUrl, newAppointment), cancellationToken);
    }

    public async Task DeleteAppointment(int id, CancellationToken cancellationToken = default)
    {
        string url = $"{_apiUrl}/{id}";

        await ApiCallerBase.SendWithResponseAsync(ApiRequest.CreateDelete(url), cancellationToken);
    }
}
