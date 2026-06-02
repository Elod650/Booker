using Booker.Models.Enums;

namespace Booker.Clients.Blazor.Server.Components.Pages.Auth;

public partial class Login
{
    private LoginRequest loginModel = new() { Email = "admin@booker.com", Password = "Admin123!" };
    private string? errorMessage;
    private bool isLoading;

    private static List<(string email, string password)> users =
    [
        new("admin@booker.com", "Admin123!"),
        new("provider@booker.com", "Provider123!"),
        new("customer@booker.com", "Customer123!"),
    ];

    private async Task HandleLogin()
    {
        this.isLoading = true;
        this.errorMessage = null;

        try
        {
            var response = await AuthApiCaller.LoginAsync(this.loginModel);

            if (string.IsNullOrWhiteSpace(response.AccessToken))
            {
                this.errorMessage = "Invalid email or password.";
                return;
            }

            await AuthStateProvider.LoginAsync(response);
            NavigationManager.NavigateTo("/calendars");
        }
        catch (ApiCallerException)
        {
            this.errorMessage = "Invalid email or password.";
        }
        catch (Exception)
        {
            this.errorMessage = "An unexpected error occurred. Please try again.";
        }
        finally
        {
            this.isLoading = false;
        }
    }

    private async Task DevLogin((string email, string password) user)
    {
        loginModel.Email = user.email;
        loginModel.Password = user.password;
        await HandleLogin();
    }
}
