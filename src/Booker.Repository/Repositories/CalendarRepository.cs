namespace Booker.Repository.Repositories;

public class CalendarRepository(AppDbContext context, IMapper mapper) : ICalendarRepository
{
    public List<CalendarDto> GetCalendars()
    {
        return mapper.Map<List<CalendarDto>>(context.Calendars);
    }
}
