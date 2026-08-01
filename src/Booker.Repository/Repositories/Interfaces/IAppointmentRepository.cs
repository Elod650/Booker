namespace Booker.Repository.Repositories.Interfaces;

public interface IAppointmentRepository
{
    Task AddAppointmentAsync(Appointment newAppointment, CancellationToken cancellationToken = default);
    Task DeleteAppointmentAsync(Appointment appointmentToDelete, CancellationToken cancellationToken = default);
    Task<Appointment?> GetAppointmentByIdAsync(
        int id,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<List<Appointment>> GetAppointmentsForCalendarAsync(
        int calendarId,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<bool> HasOverlappingAppointmentAsync(
        int calendarId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken = default
    );
}
