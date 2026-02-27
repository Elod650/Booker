namespace Booker.Services.Interfaces;

public interface IAppointmentService
{
    Task AddAppointment(AppointmentDto newAppointment);
    Task<List<AppointmentDto>> GetAppointments(int calendarId);
}
