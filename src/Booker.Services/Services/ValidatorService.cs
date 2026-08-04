namespace Booker.Services.Services;

public class ValidatorService(
    ICalendarRepository calendarRepository,
    IServiceRepository serviceRepository,
    IAppointmentRepository appointmentRepository
) : IValidatorService
{
    public async Task<bool> ValidateCalendarOwnership(int calendarId, string userId)
    {
        var calendar = await calendarRepository.GetCalendarByIdAsync(calendarId);

        if (calendar is null)
        {
            return false;
        }

        if (calendar.OwnerId != userId)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> ValidateCalendarAccess(int calendarId, string userId)
    {
        if (await ValidateCalendarOwnership(calendarId, userId))
        {
            return true;
        }

        return await calendarRepository.IsCustomerOnCalendarAsync(calendarId, userId);
    }

    public async Task<bool> ValidateServiceOwnership(int serviceId, string userId)
    {
        var service = await serviceRepository.GetServiceByIdAsync(serviceId);

        if (service is null)
        {
            return false;
        }

        return await ValidateCalendarOwnership(service.CalendarId, userId);
    }

    public async Task<bool> ValidateAppointmentOwnership(int appointmentId, string userId)
    {
        var appointment = await appointmentRepository.GetAppointmentByIdAsync(appointmentId);

        if (appointment is null)
        {
            return false;
        }

        if (appointment.UserId != userId)
        {
            return false;
        }

        return true;
    }
}
