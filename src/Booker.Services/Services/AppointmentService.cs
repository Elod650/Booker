namespace Booker.Services.Services;

public class AppointmentService(IAppointmentRepository appointmentRepository, IMapper mapper) : IAppointmentService
{
    public async Task<List<AppointmentDto>> GetAppointments(
        int calendarId,
        CancellationToken cancellationToken = default
    )
    {
        return mapper.Map<List<AppointmentDto>>(
            await appointmentRepository.GetAppointmentsForCalendarAsync(calendarId, cancellationToken)
        );
    }

    public async Task AddAppointment(AppointmentDto newAppointment, CancellationToken cancellationToken = default)
    {
        await appointmentRepository.AddAppointmentAsync(mapper.Map<Appointment>(newAppointment), cancellationToken);
    }
}
