namespace Booker.Services.Services;

public class AppointmentService(
    IAppointmentRepository appointmentRepository,
    ICalendarRepository calendarRepository,
    IServiceRepository serviceRepository,
    IMapper mapper
) : IAppointmentService
{
    public async Task<List<AppointmentDto>> GetAppointments(
        int calendarId,
        CancellationToken cancellationToken = default
    )
    {
        return mapper.Map<List<AppointmentDto>>(
            await appointmentRepository.GetAppointmentsForCalendarAsync(
                calendarId,
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task<string?> AddAppointment(
        EditAppointmentRequest newAppointment,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        var errorMessage = await ValidateBookingRules(newAppointment, cancellationToken);

        if (errorMessage is not null)
        {
            return errorMessage;
        }

        var newAppointmentEntity = new Appointment { UserId = userId };
        mapper.Map(newAppointment, newAppointmentEntity);

        newAppointmentEntity.Id = 0;

        await appointmentRepository.AddAppointmentAsync(newAppointmentEntity, cancellationToken);

        return null;
    }

    public async Task<string?> DeleteAppointment(int appointmentId, CancellationToken cancellationToken = default)
    {
        var appointmentToDelete = await appointmentRepository.GetAppointmentByIdAsync(
            appointmentId,
            asNoTracking: false,
            cancellationToken
        );

        if (appointmentToDelete is null)
        {
            return "There is no appointment with the provided Id.";
        }

        await appointmentRepository.DeleteAppointmentAsync(appointmentToDelete, cancellationToken);

        return null;
    }

    private async Task<string?> ValidateBookingRules(
        EditAppointmentRequest newAppointment,
        CancellationToken cancellationToken
    )
    {
        var calendar = await calendarRepository.GetCalendarByIdAsync(
            newAppointment.CalendarId,
            cancellationToken: cancellationToken
        );

        if (calendar is null)
        {
            return "There is no calendar with the provided Id.";
        }

        var service = await serviceRepository.GetServiceByIdAsync(
            newAppointment.ServiceId,
            cancellationToken: cancellationToken
        );

        if (service is null)
        {
            return "There is no service with the provided Id.";
        }

        if (service.CalendarId != newAppointment.CalendarId)
        {
            return "The selected service does not belong to the selected calendar.";
        }

        if (newAppointment.StartTime >= newAppointment.EndTime)
        {
            return "The start time must be earlier than the end time.";
        }

        if (newAppointment.StartTime < DateTime.Now)
        {
            return "An appointment cannot be booked in the past.";
        }

        if (newAppointment.StartTime.Date != newAppointment.EndTime.Date)
        {
            return "An appointment must start and end on the same day.";
        }

        if (
            !TimeOnly.TryParse(calendar.StartTime, out var workStart)
            || !TimeOnly.TryParse(calendar.EndTime, out var workEnd)
        )
        {
            return "The work hours of the calendar are misconfigured.";
        }

        var appointmentStart = TimeOnly.FromDateTime(newAppointment.StartTime);
        var appointmentEnd = TimeOnly.FromDateTime(newAppointment.EndTime);

        if (appointmentStart < workStart || appointmentEnd > workEnd)
        {
            return "The appointment must be within the work hours of the calendar.";
        }

        bool hasOverlap = await appointmentRepository.HasOverlappingAppointmentAsync(
            newAppointment.CalendarId,
            newAppointment.StartTime,
            newAppointment.EndTime,
            cancellationToken
        );

        if (hasOverlap)
        {
            return "The selected time slot is already booked.";
        }

        return null;
    }
}
