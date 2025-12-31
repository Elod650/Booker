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
        return await ApiCallerBase.SendAsync<List<CalendarDto>>(new ApiRequest(HttpMethod.Get, _apiUrl));
    }

    public async Task<List<CalendarDto>> GetCalendars(int calendarId)
    {
        string url = $"{_apiUrl}/{calendarId}";

        return await ApiCallerBase.SendAsync<List<CalendarDto>>(new ApiRequest(HttpMethod.Get, url));
    }
}
