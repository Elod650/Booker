namespace Booker.Clients.Blazor.Server.Components.Pages.Services;

public partial class Services
{
    private List<ServiceDto> services;
    private string currency;
    private bool isDeleteModalOpen;
    private int? serviceIdToDelete;

    protected override async Task OnInitializedAsync()
    {
        // Simulate asynchronous loading to demonstrate streaming rendering
        await Task.Delay(500);

        try
        {
            services = await ServiceApiCaller.GetServicesForUser(await AuthStateProvider.GetUserId());
            currency = await InfoApiCaller.GetCurrency();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Services)} during {nameof(OnInitializedAsync)}");
            await JSRuntime.ErrorToast("An error occured during the loading of the page");
        }
    }

    private void OnDelete(int id)
    {
        serviceIdToDelete = id;
        isDeleteModalOpen = true;
    }

    private async Task OnDeleteConfirmed()
    {
        isDeleteModalOpen = false;
        if (serviceIdToDelete is null)
        {
            return;
        }

        try
        {
            await ServiceApiCaller.DeleteServices(serviceIdToDelete.Value);

            services = await ServiceApiCaller.GetServicesForUser(await AuthStateProvider.GetUserId());

            await JSRuntime.SuccessToast("Service deleted");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Services)} during {nameof(OnDeleteConfirmed)}");
            await JSRuntime.ErrorToast("An error occured during the delete of the service");
        }
        finally
        {
            serviceIdToDelete = null;
        }
    }

    private void OnDeleteCancelled()
    {
        isDeleteModalOpen = false;
        serviceIdToDelete = null;
    }
}
