namespace Booker.Clients.Blazor.Server.Components.Pages.Services;

public partial class AddService
{
    private EditServiceViewModels model = new();

    private async Task Save()
    {
        try
        {
            await ServiceApiCaller.AddService(model.ToRequest());
            NavigationManager.NavigateTo("services");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(AddService)} during {nameof(Save)}");
            await JSRuntime.ErrorToast("An error occured during the creation of the service");
        }
    }
}
