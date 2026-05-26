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
            this.services = await this.ServiceApiCaller.GetServices();
            this.currency = await this.InfoApiCaller.GetCurrency();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Services)} during {nameof(OnInitializedAsync)}");
            await this.JSRuntime.ErrorToast("An error occured during the loading of the page");
        }
    }

    private void OnDelete(int id)
    {
        this.serviceIdToDelete = id;
        this.isDeleteModalOpen = true;
    }

    private async Task OnDeleteConfirmed()
    {
        this.isDeleteModalOpen = false;
        if (this.serviceIdToDelete is null)
        {
            return;
        }

        try
        {
            await this.ServiceApiCaller.DeleteServices(this.serviceIdToDelete.Value);

            this.services = await this.ServiceApiCaller.GetServices();

            await this.JSRuntime.SuccessToast("Service deleted");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Services)} during {nameof(OnDeleteConfirmed)}");
            await this.JSRuntime.ErrorToast("An error occured during the delete of the service");
        }
        finally
        {
            this.serviceIdToDelete = null;
        }
    }

    private void OnDeleteCancelled()
    {
        this.isDeleteModalOpen = false;
        this.serviceIdToDelete = null;
    }
}
