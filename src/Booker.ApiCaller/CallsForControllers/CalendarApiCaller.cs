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
        string url = $"{_apiUrl}/forOwner/{ownerId}";

        return await apiCallerBase.SendWithResponseAsync<List<CalendarDto>>(
            ApiRequest.CreateGet(url),
            cancellationToken: cancellationToken
        );
    }

    public async Task<List<CalendarDto>> GetCalendarsForCustomer(
        string customerId,
        CancellationToken cancellationToken = default
    )
    {
        string url = $"{_apiUrl}/forCustomer/{customerId}";

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

    public async Task DeleteCalendar(int id, CancellationToken cancellationToken = default)
    {
        string url = $"{_apiUrl}/{id}";

        await apiCallerBase.SendAsync(ApiRequest.CreateDelete(url), cancellationToken: cancellationToken);
    }

    public async Task<string?> AddCustomerToCalendar(
        AddCustomerToCalendarRequest request,
        CancellationToken cancellationToken = default
    )
    {
        string url = $"{_apiUrl}/addCustomer";

        string? response = await apiCallerBase.SendWithResponseAsync(
            ApiRequest.CreatePost(url, request),
            cancellationToken: cancellationToken
        );

        return response;
    }

    public async Task<List<UserDto>> GetCustomersForCalendar(
        int calendarId,
        CancellationToken cancellationToken = default
    )
    {
        string url = $"{_apiUrl}/{calendarId}/customers";

        return await apiCallerBase.SendWithResponseAsync<List<UserDto>>(
            ApiRequest.CreateGet(url),
            cancellationToken: cancellationToken
        );
    }

    public async Task<string?> RemoveCustomerFromCalendarAsync(
        RemoveCustomerFromCalendarRequest request,
        CancellationToken cancellationToken = default
    )
    {
        string url = $"{_apiUrl}/removeCustomer";

        string? response = await apiCallerBase.SendWithResponseAsync(
            ApiRequest.CreatePost(url, request),
            cancellationToken: cancellationToken
        );

        return response;
    }
}
