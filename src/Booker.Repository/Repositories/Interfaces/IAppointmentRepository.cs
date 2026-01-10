namespace Booker.Repository.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task AddAppointment(AppointmentDto newAppointment, CancellationToken cancellationToken = default);
        List<AppointmentDto> GetAppointments(int calendarId);
    }
}
