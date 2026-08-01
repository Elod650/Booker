namespace Booker.ApiCaller.CallsForControllers;

public class AuthApiCaller(IApiCallerBase apiCallerBase, IOptions<ApiCallerOptions> options) : IAuthApiCaller
{
    private readonly string _apiUrl = options.Value.AuthApiUrl;

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        string url = $"{this._apiUrl}/login";

        return await apiCallerBase.SendWithResponseAsync<AuthResponse>(
            ApiRequest.CreatePost(url, request),
            withBearer: false,
            cancellationToken: cancellationToken
        );
    }

    public async Task<string> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        string url = $"{this._apiUrl}/register";

        return await apiCallerBase.SendWithResponseAsync(
            ApiRequest.CreatePost(url, request),
            withBearer: false,
            cancellationToken: cancellationToken
        );
    }

    public async Task<AuthResponse> RefreshTokenAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default
    )
    {
        string url = $"{this._apiUrl}/refresh";

        return await apiCallerBase.SendWithResponseAsync<AuthResponse>(
            ApiRequest.CreatePost(url, request),
            withBearer: false,
            cancellationToken: cancellationToken
        );
    }

    public async Task LogoutAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        string url = $"{this._apiUrl}/logout";

        await apiCallerBase.SendAsync(
            ApiRequest.CreatePost(url, request),
            withBearer: false,
            cancellationToken: cancellationToken
        );
    }
}
