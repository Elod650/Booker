namespace Booker.Clients.Blazor.Server.Components.Pages.Services;

public partial class EditService
{
    [Parameter]
    public int Id { get; set; }

    private EditServiceViewModels model = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var service = await ServiceApiCaller.GetServiceById(Id);
            model = EditServiceViewModels.Create(service);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(EditService)} during {nameof(OnInitializedAsync)}");
            await JSRuntime.ErrorToast("An error occured during the loading of the service");
        }
    }

    private async Task Save()
    {
        try
        {
            await ServiceApiCaller.UpdateService(model.ToRequest());
            NavigationManager.NavigateTo("services");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(AddService)} during {nameof(Save)}");
            await JSRuntime.ErrorToast("An error occured during the creation of the service");
        }
    }
}
