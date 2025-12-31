namespace Booker.Repository.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        void AddAppointment(AppointmentDto newAppointment);
        List<AppointmentDto> GetAppointments(int calendarId);
    }
}
