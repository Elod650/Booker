namespace Booker.Services.Interfaces;

public interface IValidatorService
{
    Task<bool> ValidateCalendarOwnership(int calendarId, string userId);
}
