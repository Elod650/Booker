namespace Booker.Repository.Repositories.Interfaces;

public interface IAppointmentRepository
{
    Task AddAppointmentAsync(Appointment newAppointment, CancellationToken cancellationToken = default);
    Task<List<Appointment>> GetAppointmentsForCalendarAsync(
        int calendarId,
        CancellationToken cancellationToken = default
    );
}
