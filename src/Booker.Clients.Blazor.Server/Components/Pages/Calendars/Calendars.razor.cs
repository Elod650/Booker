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
            await JSRuntime.ErrorToast("An error occured during the loading of the page");
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
            await JSRuntime.ErrorToast("An error occured during the calendar change");
        }
    }

    private async Task OnSave(AppointmentDto newAppointment)
    {
        try
        {
            if (!IsAppointmentWithinWorkHours(newAppointment))
            {
                await JSRuntime.ErrorToast("Appointment must be within work hours");
                return;
            }

            newAppointment.CalendarId = selectedCalendarId;
            await AppointmentApiCaller.AddAppointment(newAppointment);
            appointments = await AppointmentApiCaller.GetAppointments(selectedCalendarId);
            scheduler.CloseEditor();

            await JSRuntime.SuccessToast("New appointment added");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Calendars)} during {nameof(OnSave)}");
            await JSRuntime.ErrorToast("An error occured during the creation of the new appointment");
        }
    }

    private async Task OnDelete(int id)
    {
        try
        {
            await AppointmentApiCaller.DeleteAppointment(id);

            appointments = await AppointmentApiCaller.GetAppointments(selectedCalendarId);
            scheduler.CloseEditor();

            await JSRuntime.SuccessToast("Appointment deleted");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Calendars)} during {nameof(OnDelete)}");
            await JSRuntime.ErrorToast("An error occured during the delete of the appointment");
        }
    }

    private bool IsAppointmentWithinWorkHours(AppointmentDto appointment)
    {
        if (appointment.StartTime is null || appointment.EndTime is null)
        {
            return false;
        }

        if (
            !TimeOnly.TryParse(currentCalendarStarTime, out var workStart)
            || !TimeOnly.TryParse(currentCalendarEndTime, out var workEnd)
        )
        {
            return false;
        }

        var appointmentStart = TimeOnly.FromDateTime(appointment.StartTime.Value);
        var appointmentEnd = TimeOnly.FromDateTime(appointment.EndTime.Value);

        return appointmentStart >= workStart && appointmentEnd <= workEnd;
    }

    private void OnClose()
    {
        scheduler.CloseEditor();
    }
}
