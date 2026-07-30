namespace Booker.Services.Services;

public class ValidatorService(ICalendarRepository calendarRepository, IServiceRepository serviceRepository)
    : IValidatorService
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

    public async Task<bool> ValidateServiceOwnership(int serviceId, string userId)
    {
        var service = await serviceRepository.GetServiceByIdAsync(serviceId);

        if (service is null)
        {
            return false;
        }

        return await ValidateCalendarOwnership(service.CalendarId, userId);
    }
}
