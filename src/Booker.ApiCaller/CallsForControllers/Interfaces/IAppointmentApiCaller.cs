namespace Booker.ApiCaller.CallsForControllers.Interfaces;

public interface IAppointmentApiCaller
{
    Task AddAppointment(EditAppointmentRequest newAppointment, CancellationToken cancellationToken = default);
    Task DeleteAppointment(int id, CancellationToken cancellationToken = default);
    Task<List<AppointmentDto>> GetAppointments(int calendarId, CancellationToken cancellationToken = default);
}
