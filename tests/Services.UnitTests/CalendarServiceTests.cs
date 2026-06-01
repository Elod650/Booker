using Booker.Repository.Repositories;

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

    [Test]
    public async Task AddCalendar_ShouldPass_WhenIdIsZero()
    {
        var newCalendar = Substitute.For<EditCalendarRequest>();

        var result = await calendarService.AddCalendar(newCalendar);

        await Assert.That(result).IsNull();
        await calendarRepository.Received(1).AddCalendarAsync(Arg.Any<Calendar>());
    }

    [Test]
    [Arguments(1)]
    [Arguments(-1)]
    public async Task AddCalendar_ShouldReturnError_WhenIdIsNotZero(int id)
    {
        var newCalendar = Substitute.For<EditCalendarRequest>();
        newCalendar.Id = id;

        var result = await calendarService.AddCalendar(newCalendar);

        await Assert.That(result).EqualTo("The Id has to be 0 when adding a new calendar.");
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
