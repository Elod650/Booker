namespace Booker.Clients.Blazor.Server.Components.Pages.Services;

public partial class AddService
{
    private EditServiceViewModels model = new();
    private List<CalendarDto> calendars = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            calendars = await CalendarApiCaller.GetCalendars();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(AddService)} during {nameof(OnInitializedAsync)}");
        }
    }

    private async Task Create()
    {
        try
        {
            await ServiceApiCaller.AddService(model.ToRequest());
            NavigationManager.NavigateTo("services");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(AddService)} during {nameof(Create)}");
        }
    }
}
