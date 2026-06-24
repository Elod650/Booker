namespace Booker.ApiCaller;

public interface IApiCallerBase
{
    Task SendAsync(ApiRequest request, bool withBearer = true, CancellationToken cancellationToken = default);
    Task<T> SendWithResponseAsync<T>(
        ApiRequest request,
        bool withBearer = true,
        CancellationToken cancellationToken = default
    )
        where T : class, new();
    Task<string?> SendWithResponseAsync(
        ApiRequest request,
        bool withBearer = true,
        CancellationToken cancellationToken = default
    );
    void SetBasicData(
        string refreshUrl,
        ApiCallerBase.GetAccessToken getAccessToken,
        ApiCallerBase.GetRefreshToken getRefreshToken,
        ApiCallerBase.UpdateTokens updateTokens,
        ApiCallerBase.Logout logout
    );
}
