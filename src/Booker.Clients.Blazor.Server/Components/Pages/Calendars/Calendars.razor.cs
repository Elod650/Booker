namespace Booker.Clients.Blazor.Server.Components.Pages.Calendars;

public partial class Calendars
{
    private List<CalendarDto> calendars = new();

    private bool isDeleteModalOpen;
    private int? calendarIdToDelete;

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

    private void OnDelete(int id)
    {
        calendarIdToDelete = id;
        isDeleteModalOpen = true;
    }

    private async Task OnDeleteConfirmed()
    {
        isDeleteModalOpen = false;
        if (calendarIdToDelete is null)
        {
            return;
        }

        try
        {
            await CalendarApiCaller.DeleteCalendar(calendarIdToDelete.Value);

            calendars = await CalendarApiCaller.GetCalendarsByOwnerId(await AuthStateProvider.GetUserId());

            await JSRuntime.SuccessToast("Calendar deleted");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Calendars)} during {nameof(OnDeleteConfirmed)}");
            await JSRuntime.ErrorToast("An error occured during the delete of the calendar");
        }
        finally
        {
            calendarIdToDelete = null;
        }
    }

    private void OnDeleteCancelled()
    {
        isDeleteModalOpen = false;
        calendarIdToDelete = null;
    }
}
