namespace Booker.ApiCaller.CallsForControllers;

public class CalendarApiCaller : ICalendarApiCaller
{
    private readonly string _apiUrl;

    public CalendarApiCaller(IOptions<ApiCallerOptions> options)
    {
        _apiUrl = options.Value.CalendarApiUrl;
    }

    public async Task<List<CalendarDto>> GetCalendars()
    {
        return await ApiCallerBase.SendWithResponseAsync<List<CalendarDto>>(ApiRequest.CreateGet(_apiUrl));
    }

    public async Task<List<CalendarDto>> GetCalendars(int calendarId)
    {
        string url = $"{_apiUrl}/{calendarId}";

        return await ApiCallerBase.SendWithResponseAsync<List<CalendarDto>>(ApiRequest.CreateGet(url));
    }
}
