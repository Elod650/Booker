namespace Booker.Services.Services;

public class AppointmentService(IAppointmentRepository appointmentRepository) : IAppointmentService
{
    public async Task<List<AppointmentDto>> GetAppointments(int calendarId)
    {
        return appointmentRepository.GetAppointments(calendarId);
    }

    public async Task AddAppointment(AppointmentDto newAppointment)
    {
        await appointmentRepository.AddAppointment(newAppointment);
    }
}
