namespace Booker.Clients.Blazor.Server.Components.Layout;

public partial class MainLayout
{
    private async Task HandleLogout()
    {
        await AuthStateProvider.LogoutAsync();
        NavigationManager.NavigateTo("/", forceLoad: true);
    }
}
