namespace Booker.Clients.Blazor.Server.Components.Pages.Calendars;

public partial class Calendars
{
    private List<CalendarDto> calendars = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            calendars = await CalendarApiCaller.GetCalendarsByOwnerId(await AuthStateProvider.GetUserId());
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Calendars)} during {nameof(OnInitializedAsync)}");
            await JSRuntime.ErrorToast("An error occured during the loading of the page");
        }
    }
}
