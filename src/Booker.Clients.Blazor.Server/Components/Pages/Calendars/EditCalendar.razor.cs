namespace Booker.Clients.Blazor.Server.Components.Pages.Calendars;

public partial class EditCalendar
{
    [Parameter]
    public int Id { get; set; }

    private EditCalendarViewModel model = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var calendar = await CalendarApiCaller.GetCalendarById(Id);
            model = EditCalendarViewModel.Create(calendar);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(EditCalendar)} during {nameof(OnInitializedAsync)}");
            await JSRuntime.ErrorToast("An error occured during the loading of the calendar");
        }
    }

    private async Task Save()
    {
        if (!model.IsWorkHoursOrderValid())
        {
            await JSRuntime.ErrorToast(EditCalendarViewModel.WorkHoursOrderErrorMessage);
            return;
        }

        try
        {
            await CalendarApiCaller.UpdateCalendar(model.ToRequest());
            NavigationManager.NavigateTo("calendars");
        }
        catch (ApiCallerException ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(EditCalendar)} during {nameof(Save)}");
            await JSRuntime.ErrorToast(ex.ApiResponseMessage ?? "An error occured during the saving of the calendar");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(EditCalendar)} during {nameof(Save)}");
            await JSRuntime.ErrorToast("An error occured during the saving of the calendar");
        }
    }
}
