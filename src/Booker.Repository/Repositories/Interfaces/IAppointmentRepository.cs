namespace Booker.Repository.Repositories.Interfaces;

public interface IAppointmentRepository
{
    Task AddAppointmentAsync(Appointment newAppointment, CancellationToken cancellationToken = default);
    Task DeleteAppointmentAsync(Appointment appointmentToDelete, CancellationToken cancellationToken = default);
    Task<Appointment?> GetAppointmentById(int id, CancellationToken cancellationToken = default);
    Task<List<Appointment>> GetAppointmentsForCalendarAsync(
        int calendarId,
        CancellationToken cancellationToken = default
    );
}
