namespace Booker.ApiCaller.CallsForControllers.Interfaces
{
    public interface IAppointmentApiCaller
    {
        Task AddAppointment(AppointmentDto newAppointment);
        Task<List<AppointmentDto>> GetAppointments(int calendarId);
    }
}
