namespace Booker.Services.Services;

public class AppointmentService(IAppointmentRepository appointmentRepository, IMapper mapper) : IAppointmentService
{
    public async Task<List<AppointmentDto>> GetAppointments(int calendarId)
    {
        return mapper.Map<List<AppointmentDto>>(
            await appointmentRepository.GetAppointmentsForCalendarAsync(calendarId)
        );
    }

    public async Task AddAppointment(AppointmentDto newAppointment)
    {
        await appointmentRepository.AddAppointmentAsync(mapper.Map<Appointment>(newAppointment));
    }
}
