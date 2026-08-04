namespace Services.UnitTests;

public class ValidatorServiceTests
{
    private ValidatorService validatorService = null!;
    private ICalendarRepository calendarRepository = null!;
    private IServiceRepository serviceRepository = null!;
    private IAppointmentRepository appointmentRepository = null!;

    [Before(Test)]
    public void SetUp()
    {
        SetUpRepository();

        validatorService = new ValidatorService(calendarRepository, serviceRepository, appointmentRepository);
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

    [Test]
    [Arguments(1, "1")]
    [Arguments(2, "2")]
    public async Task ValidateCalendarAccess_ShouldReturnTrue_WhenUserOwnsTheCalendar(int calendarId, string userId)
    {
        var result = await validatorService.ValidateCalendarAccess(calendarId, userId);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task ValidateCalendarAccess_ShouldReturnTrue_WhenUserIsAnInvitedCustomer()
    {
        var result = await validatorService.ValidateCalendarAccess(1, "3");

        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments(1, "4")]
    [Arguments(2, "3")]
    [Arguments(1, "user-999")]
    public async Task ValidateCalendarAccess_ShouldReturnFalse_WhenUserIsNeitherOwnerNorCustomer(
        int calendarId,
        string userId
    )
    {
        var result = await validatorService.ValidateCalendarAccess(calendarId, userId);

        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(int.MaxValue)]
    public async Task ValidateCalendarAccess_ShouldReturnFalse_WhenCalendarNotFound(int calendarId)
    {
        var result = await validatorService.ValidateCalendarAccess(calendarId, "1");

        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments(1, "3")]
    [Arguments(3, "3")]
    public async Task ValidateAppointmentOwnership_ShouldReturnTrue_WhenUserBookedTheAppointment(
        int appointmentId,
        string userId
    )
    {
        var result = await validatorService.ValidateAppointmentOwnership(appointmentId, userId);

        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments(1, "1")]
    [Arguments(3, "2")]
    public async Task ValidateAppointmentOwnership_ShouldReturnTrue_WhenUserOwnsTheCalendar(
        int appointmentId,
        string userId
    )
    {
        var result = await validatorService.ValidateAppointmentOwnership(appointmentId, userId);

        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments(1, "2")]
    [Arguments(3, "1")]
    [Arguments(1, "4")]
    [Arguments(1, "user-999")]
    public async Task ValidateAppointmentOwnership_ShouldReturnFalse_WhenUserIsNeitherBookerNorCalendarOwner(
        int appointmentId,
        string userId
    )
    {
        var result = await validatorService.ValidateAppointmentOwnership(appointmentId, userId);

        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(int.MaxValue)]
    public async Task ValidateAppointmentOwnership_ShouldReturnFalse_WhenAppointmentNotFound(int appointmentId)
    {
        var result = await validatorService.ValidateAppointmentOwnership(appointmentId, "1");

        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("")]
    [Arguments(null)]
    public async Task ValidateAppointmentOwnership_ShouldReturnFalse_WhenUserIdIsMissing(string? userId)
    {
        var result = await validatorService.ValidateAppointmentOwnership(1, userId!);

        await Assert.That(result).IsFalse();
    }

    private void SetUpRepository()
    {
        calendarRepository = Substitute.For<ICalendarRepository>();
        serviceRepository = Substitute.For<IServiceRepository>();
        appointmentRepository = Substitute.For<IAppointmentRepository>();

        calendarRepository
            .GetCalendarByIdAsync(Arg.Any<int>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.ArgAt<int>(0);
                return CalendarTestData.Calendars.FirstOrDefault(x => x.Id == id);
            });

        calendarRepository
            .IsCustomerOnCalendarAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var calendarId = callInfo.ArgAt<int>(0);
                var customerId = callInfo.ArgAt<string>(1);

                return CalendarTestData.CalendarsXCustomers.Any(x =>
                    x.CalendarId == calendarId && x.CustomerId == customerId
                );
            });

        serviceRepository
            .GetServiceByIdAsync(Arg.Any<int>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.ArgAt<int>(0);
                return ServiceTestData.Services.FirstOrDefault(x => x.Id == id);
            });

        appointmentRepository
            .GetAppointmentByIdAsync(Arg.Any<int>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.ArgAt<int>(0);
                return AppointmentTestData.Appointments.FirstOrDefault(x => x.Id == id);
            });
    }
}
