namespace Booker.ApiCaller.CallsForControllers;

public class CalendarApiCaller(IApiCallerBase apiCallerBase, IOptions<ApiCallerOptions> options) : ICalendarApiCaller
{
    private readonly string _apiUrl = options.Value.CalendarApiUrl;

    public async Task<List<CalendarDto>> GetCalendars(CancellationToken cancellationToken = default)
    {
        return await apiCallerBase.SendWithResponseAsync<List<CalendarDto>>(
            ApiRequest.CreateGet(_apiUrl),
            cancellationToken: cancellationToken
        );
    }

    public async Task<List<CalendarDto>> GetCalendarsByOwnerId(
        string ownerId,
        CancellationToken cancellationToken = default
    )
    {
        string url = $"{_apiUrl}/{ownerId}";

        return await apiCallerBase.SendWithResponseAsync<List<CalendarDto>>(
            ApiRequest.CreateGet(url),
            cancellationToken: cancellationToken
        );
    }

    public async Task AddCalendar(EditCalendarRequest newCalendar, CancellationToken cancellationToken = default)
    {
        await apiCallerBase.SendAsync(
            ApiRequest.CreatePost(_apiUrl, newCalendar),
            cancellationToken: cancellationToken
        );
    }
}
