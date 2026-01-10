namespace Booker.Repository.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task AddAppointment(AppointmentDto newAppointment);
        List<AppointmentDto> GetAppointments(int calendarId);
    }
}
