namespace Booker.Clients.Blazor.Server.Components.Pages.Calendars;

public partial class AddCalendar
{
    private EditCalendarViewModel model = new();

    private async Task Save()
    {
        if (!model.IsWorkHoursOrderValid())
        {
            await JSRuntime.ErrorToast(EditCalendarViewModel.WorkHoursOrderErrorMessage);
            return;
        }

        try
        {
            await CalendarApiCaller.AddCalendar(model.ToRequest());
            NavigationManager.NavigateTo("calendars");
        }
        catch (ApiCallerException ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(AddCalendar)} during {nameof(Save)}");
            await JSRuntime.ErrorToast(ex.ApiResponseMessage ?? "An error occured during the creation of the calendar");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(AddCalendar)} during {nameof(Save)}");
            await JSRuntime.ErrorToast("An error occured during the creation of the calendar");
        }
    }
}
