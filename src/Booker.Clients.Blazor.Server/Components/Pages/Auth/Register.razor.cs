namespace Booker.Clients.Blazor.Server.Components.Pages.Auth;

public partial class Register
{
    private RegisterRequest registerModel = new();
    private string? errorMessage;
    private string? successMessage;
    private bool isLoading;

    private async Task HandleRegister()
    {
        this.isLoading = true;
        this.errorMessage = null;
        this.successMessage = null;

        try
        {
            string response = await AuthApiCaller.RegisterAsync(this.registerModel);

            if (!string.IsNullOrWhiteSpace(response))
            {
                this.errorMessage = response;
                return;
            }

            this.successMessage = "Registration successful! Redirecting to login...";
            await Task.Delay(1500);
            NavigationManager.NavigateTo("/login");
        }
        catch (ApiCallerException ex)
        {
            this.errorMessage = ex.Message;
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
}
