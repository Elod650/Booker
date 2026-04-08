namespace Booker.Services.Interfaces;

public interface IAppointmentService
{
    Task AddAppointment(AppointmentDto newAppointment, CancellationToken cancellationToken = default);
    Task<string?> DeleteAppointment(int appointmentId, CancellationToken cancellationToken = default);
    Task<List<AppointmentDto>> GetAppointments(int calendarId, CancellationToken cancellationToken = default);
}
