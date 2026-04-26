namespace Booker.ApiCaller.CallsForControllers;

public class CalendarApiCaller(IApiCallerBase apiCallerBase, IOptions<ApiCallerOptions> options) : ICalendarApiCaller
{
    private readonly string _apiUrl = options.Value.CalendarApiUrl;

    public async Task<List<CalendarDto>> GetCalendars(CancellationToken cancellationToken = default)
    {
        var asd = ApiRequest.CreateGet(_apiUrl);
        return await apiCallerBase.SendWithResponseAsync<List<CalendarDto>>(asd, cancellationToken: cancellationToken);
    }

    public async Task<List<CalendarDto>> GetCalendars(int calendarId, CancellationToken cancellationToken = default)
    {
        string url = $"{_apiUrl}/{calendarId}";

        return await apiCallerBase.SendWithResponseAsync<List<CalendarDto>>(
            ApiRequest.CreateGet(url),
            cancellationToken: cancellationToken
        );
    }
}
