namespace Booker.Clients.Blazor.Server.Helpers
{
    public interface ICustomAuthStateProvider
    {
        Task<string?> GetAccessToken();
        Task<string?> GetRefreshToken();
        Task<string> GetUserId();
        Task LoginAsync(AuthResponse authResponse);
        Task LogoutAsync();
        Task SetTokens(string accessToken, string refreshToken);
    }
}
