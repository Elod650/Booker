namespace Booker.ApiCaller.CallsForControllers;

public class AppointmentApiCaller : IAppointmentApiCaller
{
    private readonly string _apiUrl;

    public AppointmentApiCaller(IOptions<ApiCallerOptions> options)
    {
        _apiUrl = options.Value.AppointmentApiUrl;
    }

    public async Task<List<AppointmentDto>> GetAppointments(int calendarId)
    {
        string url = $"{_apiUrl}/{calendarId}";

        return await ApiCallerBase.SendWithResponseAsync<List<AppointmentDto>>(ApiRequest.CreateGet(url));
    }

    public async Task AddAppointment(AppointmentDto newAppointment)
    {
        await ApiCallerBase.SendAsync(ApiRequest.CreatePost(_apiUrl, newAppointment));
    }
}
