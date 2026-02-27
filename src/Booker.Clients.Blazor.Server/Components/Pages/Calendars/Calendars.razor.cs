namespace Booker.Clients.Blazor.Server.Components.Pages.Calendars;

public partial class Calendars
{
    private SfSchedule<AppointmentDto> scheduler;
    private List<AppointmentDto> appointments = new();
    private List<CalendarDto> calendars = new();
    private List<ServiceDto> services = new();
    private DateTime currentDate = DateTime.Now;
    private View currentView = View.Week;
    private int selectedCalendarId;

    private string currentCalendarStarTime = "08:00";
    private string currentCalendarEndTime = "16:00";

    protected override async Task OnInitializedAsync()
    {
        try
        {
            calendars = await CalendarApiCaller.GetCalendars();
            selectedCalendarId = calendars.FirstOrDefault()?.Id ?? 0;
            appointments = await AppointmentApiCaller.GetAppointments(selectedCalendarId);
            services = await ServiceApiCaller.GetServices(selectedCalendarId);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Calendars)} during {nameof(OnInitializedAsync)}");
        }
    }

    public async Task OnCellClick(CellClickEventArgs args)
    {
        args.Cancel = true;
    }

    private async Task OnCalendarChanged(int value)
    {
        try
        {
            selectedCalendarId = value;
            appointments = await AppointmentApiCaller.GetAppointments(selectedCalendarId);
            services = await ServiceApiCaller.GetServices(selectedCalendarId);
            var selectedCalendar = calendars.First(c => c.Id == selectedCalendarId);
            currentCalendarStarTime = selectedCalendar.StartTime;
            currentCalendarEndTime = selectedCalendar.EndTime;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Calendars)} during {nameof(OnCalendarChanged)}");
        }
    }

    private async Task OnSave(AppointmentDto newAppointment)
    {
        try
        {
            newAppointment.CalendarId = selectedCalendarId;
            await AppointmentApiCaller.AddAppointment(newAppointment);
            appointments = await AppointmentApiCaller.GetAppointments(selectedCalendarId);
            scheduler.CloseEditor();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Calendars)} during {nameof(OnSave)}");
        }
    }

    private void OnClose()
    {
        scheduler.CloseEditor();
    }
}
