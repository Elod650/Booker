namespace Services.UnitTests;

public class AppointmentServiceTests
{
    private AppointmentService appointmentService = null!;
    private IAppointmentRepository appointmentRepository = null!;
    private ICalendarRepository calendarRepository = null!;
    private IServiceRepository serviceRepository = null!;
    private List<Appointment> existingAppointments = null!;

    [Before(Test)]
    public void SetUp()
    {
        SetUpRepository();
        var mapper = SetUpMapper();

        appointmentService = new AppointmentService(
            appointmentRepository,
            calendarRepository,
            serviceRepository,
            mapper
        );
    }

    [Test]
    public async Task GetAppointments_ShouldReturnDTOs()
    {
        var result = await appointmentService.GetAppointments(1);

        await Assert.That(result.Count).IsEqualTo(2);
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(int.MaxValue)]
    public async Task GetAppointments_ShouldReturnEmptyList_WhenCalendarIdIsInvalid(int calendarId)
    {
        var result = await appointmentService.GetAppointments(calendarId);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Count).IsEqualTo(0);
    }

    [Test]
    public async Task AddAppointment_ShouldPass()
    {
        var userId = "test-user-id";

        var result = await appointmentService.AddAppointment(CreateRequest(12, 13), userId);

        await Assert.That(result).IsNull();
        await appointmentRepository.Received(1).AddAppointmentAsync(Arg.Is<Appointment>(a => a.UserId == userId));
    }

    [Test]
    public async Task AddAppointment_ShouldPass_WhenTheAppointmentIsBackToBackWithAnExistingOne()
    {
        var result = await appointmentService.AddAppointment(CreateRequest(11, 12), "test-user-id");

        await Assert.That(result).IsNull();
        await appointmentRepository.Received(1).AddAppointmentAsync(Arg.Any<Appointment>());
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(int.MaxValue)]
    public async Task AddAppointment_ShouldReturnErrorMessage_WhenTheCalendarDoesNotExist(int calendarId)
    {
        var newAppointment = CreateRequest(12, 13);
        newAppointment.CalendarId = calendarId;

        var result = await appointmentService.AddAppointment(newAppointment, "test-user-id");

        await Assert.That(result).IsEqualTo("There is no calendar with the provided Id.");
        await AssertAppointmentWasNotAdded();
    }

    [Test]
    public async Task AddAppointment_ShouldReturnErrorMessage_WhenTheServiceDoesNotExist()
    {
        var newAppointment = CreateRequest(12, 13);
        newAppointment.ServiceId = int.MaxValue;

        var result = await appointmentService.AddAppointment(newAppointment, "test-user-id");

        await Assert.That(result).IsEqualTo("There is no service with the provided Id.");
        await AssertAppointmentWasNotAdded();
    }

    [Test]
    public async Task AddAppointment_ShouldReturnErrorMessage_WhenTheServiceBelongsToAnotherCalendar()
    {
        var newAppointment = CreateRequest(12, 13);
        newAppointment.ServiceId = 2;

        var result = await appointmentService.AddAppointment(newAppointment, "test-user-id");

        await Assert.That(result).IsEqualTo("The selected service does not belong to the selected calendar.");
        await AssertAppointmentWasNotAdded();
    }

    [Test]
    [Arguments(13, 12)]
    [Arguments(12, 12)]
    public async Task AddAppointment_ShouldReturnErrorMessage_WhenTheEndTimeIsNotAfterTheStartTime(
        int startHour,
        int endHour
    )
    {
        var result = await appointmentService.AddAppointment(CreateRequest(startHour, endHour), "test-user-id");

        await Assert.That(result).IsEqualTo("The start time must be earlier than the end time.");
        await AssertAppointmentWasNotAdded();
    }

    [Test]
    public async Task AddAppointment_ShouldReturnErrorMessage_WhenTheAppointmentIsInThePast()
    {
        var newAppointment = CreateRequest(12, 13);
        newAppointment.StartTime = newAppointment.StartTime.AddDays(-2);
        newAppointment.EndTime = newAppointment.EndTime.AddDays(-2);

        var result = await appointmentService.AddAppointment(newAppointment, "test-user-id");

        await Assert.That(result).IsEqualTo("An appointment cannot be booked in the past.");
        await AssertAppointmentWasNotAdded();
    }

    [Test]
    public async Task AddAppointment_ShouldReturnErrorMessage_WhenTheAppointmentSpansMultipleDays()
    {
        var newAppointment = CreateRequest(16, 9);
        newAppointment.EndTime = newAppointment.EndTime.AddDays(1);

        var result = await appointmentService.AddAppointment(newAppointment, "test-user-id");

        await Assert.That(result).IsEqualTo("An appointment must start and end on the same day.");
        await AssertAppointmentWasNotAdded();
    }

    [Test]
    [Arguments(6, 7)]
    [Arguments(7, 9)]
    [Arguments(16, 18)]
    [Arguments(20, 21)]
    public async Task AddAppointment_ShouldReturnErrorMessage_WhenTheAppointmentIsOutsideTheWorkHours(
        int startHour,
        int endHour
    )
    {
        var result = await appointmentService.AddAppointment(CreateRequest(startHour, endHour), "test-user-id");

        await Assert.That(result).IsEqualTo("The appointment must be within the work hours of the calendar.");
        await AssertAppointmentWasNotAdded();
    }

    [Test]
    [Arguments(10, 11)]
    [Arguments(9, 11)]
    [Arguments(10, 12)]
    [Arguments(9, 12)]
    public async Task AddAppointment_ShouldReturnErrorMessage_WhenTheAppointmentOverlapsAnExistingOne(
        int startHour,
        int endHour
    )
    {
        var result = await appointmentService.AddAppointment(CreateRequest(startHour, endHour), "test-user-id");

        await Assert.That(result).IsEqualTo("The selected time slot is already booked.");
        await AssertAppointmentWasNotAdded();
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    public async Task DeleteAppointmentAsync_ShouldDeleteAppointment_WhenIdIsValid(int id)
    {
        var result = await appointmentService.DeleteAppointment(id);

        await Assert.That(result).IsNull();
        await appointmentRepository.Received(1).DeleteAppointmentAsync(Arg.Is<Appointment>(a => a.Id == id));
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(int.MaxValue)]
    public async Task DeleteAppointmentAsync_ShouldReturnErrorMessage_WhenThereIsNoAppointmentWithId(int id)
    {
        var result = await appointmentService.DeleteAppointment(id);

        await Assert.That(result).IsEqualTo("There is no appointment with the provided Id.");
    }

    /// <summary>
    /// Creates a valid request for calendar 1 (work hours 08:00-17:00) on the next day, at the given hours.
    /// </summary>
    private static EditAppointmentRequest CreateRequest(int startHour, int endHour)
    {
        return new EditAppointmentRequest
        {
            CalendarId = 1,
            ServiceId = 1,
            StartTime = NextDayAt(startHour),
            EndTime = NextDayAt(endHour),
        };
    }

    private static DateTime NextDayAt(int hour)
    {
        return DateTime.Now.Date.AddDays(1).AddHours(hour);
    }

    private async Task AssertAppointmentWasNotAdded()
    {
        await appointmentRepository.DidNotReceive().AddAppointmentAsync(Arg.Any<Appointment>());
    }

    private void SetUpRepository()
    {
        appointmentRepository = Substitute.For<IAppointmentRepository>();
        calendarRepository = Substitute.For<ICalendarRepository>();
        serviceRepository = Substitute.For<IServiceRepository>();

        // The seeded appointments use times relative to DateTime.Now, so the booking rule tests
        // rely on this extra appointment with a fixed slot on the next day.
        existingAppointments =
        [
            .. AppointmentTestData.Appointments,
            new Appointment
            {
                Id = 10,
                CalendarId = 1,
                ServiceId = 1,
                StartTime = NextDayAt(10),
                EndTime = NextDayAt(11),
                UserId = "1",
            },
        ];

        calendarRepository
            .GetCalendarByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.ArgAt<int>(0);
                return CalendarTestData.Calendars.FirstOrDefault(x => x.Id == id);
            });

        serviceRepository
            .GetServiceByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.ArgAt<int>(0);
                return ServiceTestData.Services.FirstOrDefault(x => x.Id == id);
            });

        appointmentRepository
            .HasOverlappingAppointmentAsync(
                Arg.Any<int>(),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo =>
            {
                var calendarId = callInfo.ArgAt<int>(0);
                var startTime = callInfo.ArgAt<DateTime>(1);
                var endTime = callInfo.ArgAt<DateTime>(2);

                return existingAppointments.Any(a =>
                    a.CalendarId == calendarId && a.StartTime < endTime && startTime < a.EndTime
                );
            });

        appointmentRepository
            .GetAppointmentsForCalendarAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var calendarId = callInfo.ArgAt<int>(0);
                return AppointmentTestData.Appointments.Where(a => a.CalendarId == calendarId).ToList();
            });

        appointmentRepository
            .GetAppointmentByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.ArgAt<int>(0);
                return AppointmentTestData.Appointments.FirstOrDefault(a => a.Id == id);
            });
    }

    private IMapper SetUpMapper()
    {
        var mapper = Substitute.For<IMapper>();

        mapper
            .Map<List<AppointmentDto>>(Arg.Any<List<Appointment>>())
            .Returns(callInfo =>
            {
                var entities = callInfo.ArgAt<List<Appointment>>(0);
                var dtos = new List<AppointmentDto>();

                foreach (var entity in entities)
                {
                    dtos.Add(
                        new AppointmentDto
                        {
                            Id = entity.Id,
                            CalendarId = entity.CalendarId,
                            ServiceId = entity.ServiceId,
                            StartTime = entity.StartTime,
                            EndTime = entity.EndTime,
                            IsReadonly = entity.IsReadonly,
                        }
                    );
                }

                return dtos;
            });

        return mapper;
    }
}
