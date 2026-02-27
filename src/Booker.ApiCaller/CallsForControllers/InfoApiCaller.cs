namespace Booker.ApiCaller.CallsForControllers;

public class InfoApiCaller : IInfoApiCaller
{
    private readonly string _apiUrl;

    public InfoApiCaller(IOptions<ApiCallerOptions> options)
    {
        _apiUrl = options.Value.InfoApiUrl;
    }

    public async Task<string> GetCurrency(CancellationToken cancellationToken = default)
    {
        string url = $"{_apiUrl}/currency";

        var currency = await ApiCallerBase.SendWithResponseAsync(ApiRequest.CreateGet(url), cancellationToken);

        return currency.Replace("\"", "");
    }
}
