namespace Services.UnitTests;

public class CalendarServiceTests
{
    private CalendarService calendarService = null!;
    private ICalendarRepository calendarRepository = null!;

    [Before(Test)]
    public void SetUp()
    {
        SetUpRepository();
        var mapper = SetUpMapper();

        calendarService = new CalendarService(calendarRepository, mapper);
    }

    [Test]
    public async Task GetAppointments_ShouldReturnDTOs()
    {
        var result = await calendarService.GetCalendars();

        await Assert.That(result.Count).IsEqualTo(2);
    }

    private void SetUpRepository()
    {
        calendarRepository = Substitute.For<ICalendarRepository>();

        calendarRepository.GetCalendarsAsync(Arg.Any<CancellationToken>()).Returns(CalendarTestData.Calendars);
    }

    private IMapper SetUpMapper()
    {
        var mapper = Substitute.For<IMapper>();

        mapper
            .Map<List<CalendarDto>>(Arg.Any<List<Calendar>>())
            .Returns(callInfo =>
            {
                var entities = callInfo.ArgAt<List<Calendar>>(0);
                var dtos = new List<CalendarDto>();

                foreach (var entity in entities)
                {
                    dtos.Add(
                        new CalendarDto
                        {
                            Id = entity.Id,
                            Name = entity.Name,
                            StartTime = entity.StartTime,
                            EndTime = entity.EndTime,
                        }
                    );
                }

                return dtos;
            });

        return mapper;
    }
}
