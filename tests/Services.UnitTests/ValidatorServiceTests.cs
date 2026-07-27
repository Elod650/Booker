namespace Services.UnitTests;

public class ValidatorServiceTests
{
    private ValidatorService validatorService = null!;
    private ICalendarRepository calendarRepository = null!;

    [Before(Test)]
    public void SetUp()
    {
        SetUpRepository();

        validatorService = new ValidatorService(calendarRepository);
    }

    [Test]
    [Arguments(1, "1")]
    [Arguments(2, "2")]
    public async Task ValidateCalendarOwnership_ShouldReturnTrue_WhenUserOwnsTheCalendar(int calendarId, string userId)
    {
        var result = await validatorService.ValidateCalendarOwnership(calendarId, userId);

        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(int.MaxValue)]
    public async Task ValidateCalendarOwnership_ShouldReturnFalse_WhenCalendarNotFound(int calendarId)
    {
        var result = await validatorService.ValidateCalendarOwnership(calendarId, "1");

        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments(1, "2")]
    [Arguments(2, "1")]
    [Arguments(1, "user-999")]
    public async Task ValidateCalendarOwnership_ShouldReturnFalse_WhenUserIsNotTheOwner(int calendarId, string userId)
    {
        var result = await validatorService.ValidateCalendarOwnership(calendarId, userId);

        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("")]
    [Arguments(null)]
    public async Task ValidateCalendarOwnership_ShouldReturnFalse_WhenUserIdIsMissing(string? userId)
    {
        var result = await validatorService.ValidateCalendarOwnership(1, userId!);

        await Assert.That(result).IsFalse();
    }

    private void SetUpRepository()
    {
        calendarRepository = Substitute.For<ICalendarRepository>();

        calendarRepository
            .GetCalendarByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.ArgAt<int>(0);
                return CalendarTestData.Calendars.FirstOrDefault(x => x.Id == id);
            });
    }
}
