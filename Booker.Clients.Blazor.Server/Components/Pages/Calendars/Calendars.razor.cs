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

    protected override void OnInitialized()
    {
        calendars = CalendarRepository.GetCalendars();
        selectedCalendarId = calendars.FirstOrDefault()?.Id ?? 0;
        appointments = AppointmentRepository.GetAppointments(selectedCalendarId);
        services = ServiceRepository.GetServices(selectedCalendarId);
    }

    public async Task OnCellClick(CellClickEventArgs args)
    {
        args.Cancel = true;
    }

    private async Task OnCalendarChanged(int value)
    {
        selectedCalendarId = value;
        appointments = AppointmentRepository.GetAppointments(selectedCalendarId);
        services = ServiceRepository.GetServices(selectedCalendarId);
        var selectedCalendar = calendars.First(c => c.Id == selectedCalendarId);
        currentCalendarStarTime = selectedCalendar.StartTime;
        currentCalendarEndTime = selectedCalendar.EndTime;
        await scheduler.RefreshAsync();
    }

    private async Task OnSave(AppointmentDto newAppointment)
    {
        newAppointment.CalendarId = selectedCalendarId;
        AppointmentRepository.AddAppointment(newAppointment);
        appointments = AppointmentRepository.GetAppointments(selectedCalendarId);
        scheduler.CloseEditor();
        await scheduler.RefreshAsync();
    }

    private void OnClose()
    {
        scheduler.CloseEditor();
    }
}
