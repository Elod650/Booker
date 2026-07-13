namespace Booker.Services.Services;

public class ValidatorService(ICalendarRepository calendarRepository) : IValidatorService
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
}
