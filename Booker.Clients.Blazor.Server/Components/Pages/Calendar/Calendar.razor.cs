namespace Booker.Clients.Blazor.Server.Components.Pages.Calendar;

public partial class Calendar
{
    private SfSchedule<AppointmentDto> scheduler;
    private List<AppointmentDto> appointments = new();
    private List<CalendarDto> calendars = new();
    private List<ServiceDto> services = new();
    private DateTime currentDate = DateTime.Now;
    private View currentView = View.Week;
    private int selectedCalendarId;

    private async Task OnCalendarChanged(int value)
    {
        selectedCalendarId = value;
        appointments = AppointmentRepository.GetAppointments(selectedCalendarId);
        services = ServiceRepository.GetServices(selectedCalendarId);
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

    protected override void OnInitialized()
    {
        calendars = CalendarRepository.GetCalendars();
        selectedCalendarId = calendars.FirstOrDefault()?.Id ?? 0;
        appointments = AppointmentRepository.GetAppointments(selectedCalendarId);
        services = ServiceRepository.GetServices(selectedCalendarId);
    }
}
