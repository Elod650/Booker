namespace Booker.Clients.Blazor.Server.Components.Pages.Services;

public partial class AddService
{
    private EditServiceViewModels model = new();
    private List<CalendarDto> calendars = new();

    protected override async Task OnInitializedAsync()
    {
        calendars = await CalendarApiCaller.GetCalendars();
    }

    private async Task Create()
    {
        await ServiceApiCaller.AddService(model.ToRequest());
        NavigationManager.NavigateTo("services");
    }
}
