namespace Booker.Clients.Blazor.Server.Components.Pages.Booking;

public partial class Booking
{
    private SfSchedule<SchedulerAppointmentViewModel> scheduler;
    private IEnumerable<SchedulerAppointmentViewModel> appointments;
    private List<CalendarDto> calendars = new();
    private List<ServiceDto> services = new();
    private DateTime currentDate = DateTime.Now;
    private View currentView = View.Week;
    private int selectedCalendarId;

    private string currentCalendarStarTime = "08:00";
    private string currentCalendarEndTime = "16:00";
    private bool isDeleteModalOpen;
    private int? appointmentIdToDelete;
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            calendars = await CalendarApiCaller.GetCalendarsForCustomer();
            selectedCalendarId = calendars.FirstOrDefault()?.Id ?? 0;
            appointments = SchedulerAppointmentViewModel.Create(
                await AppointmentApiCaller.GetAppointments(selectedCalendarId)
            );
            services = await ServiceApiCaller.GetServicesForCalendar(selectedCalendarId);

            isLoading = false;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Booking)} during {nameof(OnInitializedAsync)}");
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
            appointments = SchedulerAppointmentViewModel.Create(
                await AppointmentApiCaller.GetAppointments(selectedCalendarId)
            );
            services = await ServiceApiCaller.GetServicesForCalendar(selectedCalendarId);
            var selectedCalendar = calendars.First(c => c.Id == selectedCalendarId);
            currentCalendarStarTime = selectedCalendar.StartTime;
            currentCalendarEndTime = selectedCalendar.EndTime;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Booking)} during {nameof(OnCalendarChanged)}");
            await JSRuntime.ErrorToast("An error occured during the calendar change");
        }
    }

    private async Task OnSave(SchedulerAppointmentViewModel newAppointment)
    {
        try
        {
            if (!IsAppointmentWithinWorkHours(newAppointment))
            {
                await JSRuntime.ErrorToast("Appointment must be within work hours");
                return;
            }

            newAppointment.CalendarId = selectedCalendarId;
            await AppointmentApiCaller.AddAppointment(newAppointment.ToRequest());
            appointments = SchedulerAppointmentViewModel.Create(
                await AppointmentApiCaller.GetAppointments(selectedCalendarId)
            );
            scheduler.CloseEditor();

            await JSRuntime.SuccessToast("New appointment added");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Booking)} during {nameof(OnSave)}");
            await JSRuntime.ErrorToast("An error occured during the creation of the new appointment");
        }
    }

    private void OnDelete(int id)
    {
        this.appointmentIdToDelete = id;
        this.isDeleteModalOpen = true;
    }

    private async Task OnDeleteConfirmed()
    {
        this.isDeleteModalOpen = false;
        if (this.appointmentIdToDelete is null)
        {
            return;
        }

        try
        {
            await AppointmentApiCaller.DeleteAppointment(this.appointmentIdToDelete.Value);

            this.appointments = SchedulerAppointmentViewModel.Create(
                await AppointmentApiCaller.GetAppointments(selectedCalendarId)
            );
            this.scheduler.CloseEditor();

            await JSRuntime.SuccessToast("Appointment deleted");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"An error occurred in {nameof(Booking)} during {nameof(OnDeleteConfirmed)}");
            await JSRuntime.ErrorToast("An error occured during the delete of the appointment");
        }
        finally
        {
            this.appointmentIdToDelete = null;
        }
    }

    private void OnDeleteCancelled()
    {
        this.isDeleteModalOpen = false;
        this.appointmentIdToDelete = null;
    }

    private bool IsAppointmentWithinWorkHours(SchedulerAppointmentViewModel appointment)
    {
        if (
            !TimeOnly.TryParse(currentCalendarStarTime, CultureInfo.InvariantCulture, out var workStart)
            || !TimeOnly.TryParse(currentCalendarEndTime, CultureInfo.InvariantCulture, out var workEnd)
        )
        {
            return false;
        }

        var appointmentStart = TimeOnly.FromDateTime(appointment.StartTime);
        var appointmentEnd = TimeOnly.FromDateTime(appointment.EndTime);

        return appointmentStart >= workStart && appointmentEnd <= workEnd;
    }

    private void OnClose()
    {
        scheduler.CloseEditor();
    }
}
