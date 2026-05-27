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

    public async Task AddAppointment(
        EditAppointmentRequest newAppointment,
        CancellationToken cancellationToken = default
    )
    {
        await appointmentRepository.AddAppointmentAsync(mapper.Map<Appointment>(newAppointment), cancellationToken);
    }

    public async Task<string?> DeleteAppointment(int appointmentId, CancellationToken cancellationToken = default)
    {
        var appointmentToDelete = await appointmentRepository.GetAppointmentByIdAsync(appointmentId, cancellationToken);

        if (appointmentToDelete is null)
        {
            return "There is no appointment with the provided Id.";
        }

        await appointmentRepository.DeleteAppointmentAsync(appointmentToDelete, cancellationToken);

        return null;
    }
}
