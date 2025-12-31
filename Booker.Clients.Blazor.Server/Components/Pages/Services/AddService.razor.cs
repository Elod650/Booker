namespace Booker.Clients.Blazor.Server.Components.Pages.Services;

public partial class AddService
{
    private EditServiceViewModels model = new();
    private List<CalendarDto> calendars = new();

    protected override void OnInitialized()
    {
        calendars = CalendarRepository.GetCalendars();
    }

    private async Task Create()
    {
        ServiceRepository.AddServices(model.ToRequest());
        NavigationManager.NavigateTo("services");
    }
}
