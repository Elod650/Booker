namespace Booker.Services.Interfaces;

public interface IValidatorService
{
    Task<bool> ValidateAppointmentOwnership(int appointmentId, string userId);
    Task<bool> ValidateCalendarOwnership(int calendarId, string userId);
    Task<bool> ValidateServiceOwnership(int serviceId, string userId);
}
