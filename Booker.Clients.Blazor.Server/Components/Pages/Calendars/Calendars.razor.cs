using Syncfusion.Blazor.Schedule.Internal;

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
        calendars = await CalendarApiCaller.GetCalendars();
        selectedCalendarId = calendars.FirstOrDefault()?.Id ?? 0;
        appointments = await AppointmentApiCaller.GetAppointments(selectedCalendarId);
        services = await ServiceApiCaller.GetServices(selectedCalendarId);
    }

    public async Task OnCellClick(CellClickEventArgs args)
    {
        args.Cancel = true;
    }

    private async Task OnCalendarChanged(int value)
    {
        selectedCalendarId = value;
        appointments = await AppointmentApiCaller.GetAppointments(selectedCalendarId);
        services = await ServiceApiCaller.GetServices(selectedCalendarId);
        var selectedCalendar = calendars.First(c => c.Id == selectedCalendarId);
        currentCalendarStarTime = selectedCalendar.StartTime;
        currentCalendarEndTime = selectedCalendar.EndTime;
        await scheduler.RefreshAsync();
    }

    private async Task OnSave(AppointmentDto newAppointment)
    {
        newAppointment.CalendarId = selectedCalendarId;
        await AppointmentApiCaller.AddAppointment(newAppointment);
        appointments = await AppointmentApiCaller.GetAppointments(selectedCalendarId);
        scheduler.CloseEditor();
        await scheduler.RefreshAsync();
    }

    private void OnClose()
    {
        scheduler.CloseEditor();
    }
}
