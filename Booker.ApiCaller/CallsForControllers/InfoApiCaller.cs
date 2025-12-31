namespace Booker.ApiCaller.CallsForControllers;

public class InfoApiCaller : IInfoApiCaller
{
    private readonly string _apiUrl;

    public InfoApiCaller(IOptions<ApiCallerOptions> options)
    {
        _apiUrl = options.Value.InfoApiUrl;
    }

    public async Task<string> GetCurrency()
    {
        string url = $"{_apiUrl}/currency";

        return await ApiCallerBase.SendAsync<string>(new ApiRequest(HttpMethod.Get, url));
    }
}
