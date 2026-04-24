namespace Booker.Clients.Blazor.Server.Helpers
{
    public interface ICustomAuthStateProvider
    {
        Task LoginAsync(AuthResponse authResponse);
        Task LogoutAsync();
    }
}
