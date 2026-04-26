namespace Booker.ApiCaller.CallsForControllers;

public class InfoApiCaller(IApiCallerBase apiCallerBase, IOptions<ApiCallerOptions> options) : IInfoApiCaller
{
    private readonly string _apiUrl = options.Value.InfoApiUrl;

    public async Task<string> GetCurrency(CancellationToken cancellationToken = default)
    {
        string url = $"{_apiUrl}/currency";

        var currency = await apiCallerBase.SendWithResponseAsync(
            ApiRequest.CreateGet(url),
            cancellationToken: cancellationToken
        );

        return currency.Replace("\"", "");
    }
}
