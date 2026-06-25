namespace Booker.Clients.Blazor.Server.ViewModels;

public class SchedulerAppointmentViewModel
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsReadonly { get; set; }

    public int ServiceId { get; set; }
    public int CalendarId { get; set; }
    public string? BookingUser { get; set; }

    public EditAppointmentRequest ToRequest()
    {
        return new EditAppointmentRequest
        {
            Id = Id,
            StartTime = StartTime,
            EndTime = EndTime,
            IsReadonly = IsReadonly,
            ServiceId = ServiceId,
            CalendarId = CalendarId,
        };
    }

    public static IEnumerable<SchedulerAppointmentViewModel> Create(List<AppointmentDto> appointments)
    {
        foreach (AppointmentDto appointment in appointments)
        {
            yield return new SchedulerAppointmentViewModel
            {
                Id = appointment.Id,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                IsReadonly = appointment.IsReadonly,
                ServiceId = appointment.ServiceId,
                CalendarId = appointment.CalendarId,
                BookingUser = appointment.BookingUser,
            };
        }
    }
}
