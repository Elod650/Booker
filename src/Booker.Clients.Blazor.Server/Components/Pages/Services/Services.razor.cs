namespace Booker.Clients.Blazor.Server.Components.Pages.Services;

public partial class Services
{
    private List<ServiceDto> services;
    private string currency;

    protected override async Task OnInitializedAsync()
    {
        // Simulate asynchronous loading to demonstrate streaming rendering
        await Task.Delay(500);

        try
        {
            services = await ServiceApiCaller.GetServices();
            currency = await InfoApiCaller.GetCurrency();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Services)} during {nameof(OnInitializedAsync)}");
        }
    }
}
