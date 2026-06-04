namespace Booker.Clients.Blazor.Server.Components.Pages.Services.Components;

public partial class EditServiceComponent
{
    [Parameter]
    public EditServiceViewModels Model { get; set; } = new();

    [Parameter, EditorRequired]
    public EventCallback OnSaveEvent { get; set; }

    private List<CalendarDto> calendars = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            calendars = await CalendarApiCaller.GetCalendarsByOwnerId(await AuthStateProvider.GetUserId());
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(AddService)} during {nameof(OnInitializedAsync)}");
            await JSRuntime.ErrorToast("An error occured during the loading of the page");
        }
    }
}
