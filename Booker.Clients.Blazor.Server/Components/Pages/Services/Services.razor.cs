namespace Booker.Clients.Blazor.Server.Components.Pages.Services;

public partial class Services
{
    private List<ServiceDto> services;
    private string currency;

    protected override async Task OnInitializedAsync()
    {
        // Simulate asynchronous loading to demonstrate streaming rendering
        await Task.Delay(500);

        services = ServiceRepository.GetServices();
        currency = InfoRepository.GetCurrency();
    }
}
