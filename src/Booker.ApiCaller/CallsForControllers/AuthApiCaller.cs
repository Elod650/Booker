namespace Booker.ApiCaller.CallsForControllers;

public class AuthApiCaller : IAuthApiCaller
{
    private readonly string _apiUrl;

    public AuthApiCaller(IOptions<ApiCallerOptions> options)
    {
        this._apiUrl = options.Value.AuthApiUrl;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        string url = $"{this._apiUrl}/login";

        return await ApiCallerBase.SendWithResponseAsync<AuthResponse>(
            ApiRequest.CreatePost(url, request),
            cancellationToken
        );
    }

    public async Task<string> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        string url = $"{this._apiUrl}/register";

        return await ApiCallerBase.SendWithResponseAsync(ApiRequest.CreatePost(url, request), cancellationToken);
    }

    public async Task<AuthResponse> RefreshTokenAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default
    )
    {
        string url = $"{this._apiUrl}/refresh";

        return await ApiCallerBase.SendWithResponseAsync<AuthResponse>(
            ApiRequest.CreatePost(url, request),
            cancellationToken
        );
    }
}
