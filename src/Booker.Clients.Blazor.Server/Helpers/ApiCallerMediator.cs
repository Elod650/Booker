namespace Booker.Clients.Blazor.Server.Helpers;

public class ApiCallerMediator
{
    public ApiCallerMediator(
        IConfiguration configuration,
        IApiCallerBase apiCallBase,
        ICustomAuthStateProvider authStateProvider
    )
    {
        apiCallBase.SetBasicData(
            configuration["ApiCallerOptions:TokenRefreshUrl"] ?? string.Empty,
            authStateProvider.GetAccessToken,
            authStateProvider.GetRefreshToken,
            authStateProvider.SetTokens,
            authStateProvider.LogoutAsync
        );
    }
}
