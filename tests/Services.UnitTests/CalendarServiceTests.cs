namespace Services.UnitTests;

public class CalendarServiceTests
{
    private CalendarService calendarService = null!;
    private ICalendarRepository calendarRepository = null!;
    private IAppointmentRepository appointmentRepository = null!;
    private UserManager<ApplicationUser> userManager = null!;

    [Before(Test)]
    public void SetUp()
    {
        SetUpRepository();
        SetUpAppointmentRepository();
        SetUpUserManager();
        var mapper = SetUpMapper();

        calendarService = new CalendarService(calendarRepository, appointmentRepository, userManager, mapper);
    }

    [Test]
    public async Task GetCalendars_ShouldReturnCalendars_WhenCalendarsExist()
    {
        var result = await calendarService.GetCalendars();

        await Assert.That(result.Count).IsEqualTo(2);
    }

    [Test]
    public async Task GetCalendars_ShouldReturnEmptyList_WhenNoCalendarsExist()
    {
        calendarRepository
            .GetCalendarsAsync(
                Arg.Any<Expression<Func<Calendar, bool>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>()
            )
            .Returns([]);

        var result = await calendarService.GetCalendars();

        await Assert.That(result).IsEmpty();
    }

    [Test]
    public async Task AddCalendar_ShouldPass_WhenIdIsNull()
    {
        var newCalendar = new EditCalendarRequest
        {
            Name = "New Calendar",
            StartTime = "08:00",
            EndTime = "17:00",
        };

        var result = await calendarService.AddCalendar(newCalendar, "user-1");

        await Assert.That(result).IsNull();
        await calendarRepository
            .Received(1)
            .AddCalendarAsync(
                Arg.Is<Calendar>(c =>
                    c.OwnerId == "user-1" && c.Name == "New Calendar" && c.StartTime == "08:00" && c.EndTime == "17:00"
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Test]
    [Arguments(0)]
    [Arguments(1)]
    [Arguments(-1)]
    public async Task AddCalendar_ShouldReturnError_WhenIdIsNotNull(int id)
    {
        var newCalendar = new EditCalendarRequest
        {
            Id = id,
            Name = "New Calendar",
            StartTime = "08:00",
            EndTime = "17:00",
        };

        var result = await calendarService.AddCalendar(newCalendar, "user-1");

        await Assert.That(result).IsEqualTo("The Id has to be null when adding a new calendar.");
        await calendarRepository.DidNotReceiveWithAnyArgs().AddCalendarAsync(default!, default);
    }

    [Test]
    [Arguments("1")]
    [Arguments("2")]
    public async Task GetCalendarsByOwnerId_ShouldReturnCorrectResult_WhenOwnerHasCalendar(string ownerId)
    {
        var result = await calendarService.GetCalendarsByOwnerId(ownerId);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Count).IsEqualTo(1);
    }

    [Test]
    [Arguments("0")]
    [Arguments(null)]
    public async Task GetCalendarsByOwnerId_ShouldReturnEmptyList_WhenOwnerHasNoCalendar(string? ownerId)
    {
        var result = await calendarService.GetCalendarsByOwnerId(ownerId);

        await Assert.That(result.Count).IsEqualTo(0);
    }

    [Test]
    public async Task AddCustomerToCalendar_ShouldReturnError_WhenUserNotFound()
    {
        var request = new AddCustomerToCalendarRequest { CustomerEmail = "notfound@booker.com", CalendarId = 1 };

        var result = await calendarService.AddCustomerToCalendar(request);

        await Assert.That(result).EqualTo("There is no user with this email.");
    }

    [Test]
    public async Task AddCustomerToCalendar_ShouldReturnError_WhenCalendarNotFound()
    {
        var request = new AddCustomerToCalendarRequest { CustomerEmail = "test@booker.com", CalendarId = 999 };

        var result = await calendarService.AddCustomerToCalendar(request);

        await Assert.That(result).EqualTo("There is no calendar with the provided Id.");
    }

    [Test]
    public async Task AddCustomerToCalendar_ShouldReturnError_WhenUserAlreadyAdded()
    {
        var request = new AddCustomerToCalendarRequest { CustomerEmail = "test@booker.com", CalendarId = 1 };

        var user = UserTestData.Users.First();
        calendarRepository
            .GetCustomersForCalendarAsync(1, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns([user]);

        var result = await calendarService.AddCustomerToCalendar(request);

        await Assert.That(result).EqualTo("The user is already added to the clanedar.");
    }

    [Test]
    public async Task AddCustomerToCalendar_ShouldPass_WhenCustomerAddedSuccessfully()
    {
        var request = new AddCustomerToCalendarRequest { CustomerEmail = "test@booker.com", CalendarId = 1 };

        calendarRepository.GetCustomersForCalendarAsync(1, Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns([]);

        var result = await calendarService.AddCustomerToCalendar(request);

        await Assert.That(result).IsNull();
        await calendarRepository
            .Received(1)
            .AddCustomerToCalendarAsync(
                Arg.Is<CalendarsXCustomers>(c => c.CalendarId == 1 && c.CustomerId == "user-1"),
                Arg.Any<CancellationToken>()
            );
    }

    [Test]
    public async Task GetCustomersForCalendar_ShouldReturnNull_WhenCalendarNotFound()
    {
        var result = await calendarService.GetCustomersForCalendar(999);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task GetCustomersForCalendar_ShouldReturnCustomers_WhenCalendarExists()
    {
        var user = UserTestData.Users.First();
        calendarRepository
            .GetCustomersForCalendarAsync(1, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns([user]);

        var result = await calendarService.GetCustomersForCalendar(1);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Count).IsEqualTo(1);
        await Assert.That(result[0].Email).IsEqualTo("test@booker.com");
    }

    [Test]
    public async Task RemoveCustomerFromCalendar_ShouldReturnError_WhenUserNotFound()
    {
        var request = new RemoveCustomerFromCalendarRequest { CustomerEmail = "notfound@booker.com", CalendarId = 1 };

        var result = await calendarService.RemoveCustomerFromCalendar(request);

        await Assert.That(result).EqualTo("There is no user with this email.");
    }

    [Test]
    public async Task RemoveCustomerFromCalendar_ShouldReturnError_WhenCalendarNotFound()
    {
        var request = new RemoveCustomerFromCalendarRequest { CustomerEmail = "test@booker.com", CalendarId = 999 };

        var result = await calendarService.RemoveCustomerFromCalendar(request);

        await Assert.That(result).EqualTo("There is no calendar with the provided Id.");
    }

    [Test]
    public async Task RemoveCustomerFromCalendar_ShouldPass_WhenCustomerRemovedSuccessfully()
    {
        var request = new RemoveCustomerFromCalendarRequest { CustomerEmail = "test@booker.com", CalendarId = 1 };

        var result = await calendarService.RemoveCustomerFromCalendar(request);

        await Assert.That(result).IsNull();
        await calendarRepository.Received(1).RemoveCustomerFromCalendarAsync("user-1", 1, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetCalendarsForCustomer_ShouldReturnCalendars_WhenCustomerHasCalendars()
    {
        var result = await calendarService.GetCalendarsForCustomer("user-1");

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Count).IsEqualTo(1);
    }

    [Test]
    public async Task GetCalendarsForCustomer_ShouldReturnEmptyList_WhenCustomerHasNoCalendars()
    {
        var result = await calendarService.GetCalendarsForCustomer("user-none");

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Count).IsEqualTo(0);
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    public async Task DeleteCalendar_ShouldDeleteCalendar_WhenIdIsValid(int calendarId)
    {
        var result = await calendarService.DeleteCalendar(calendarId);

        await Assert.That(result).IsNull();
        await calendarRepository.Received(1).DeleteCalendarAsync(Arg.Is<Calendar>(c => c.Id == calendarId));
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(int.MaxValue)]
    public async Task DeleteCalendar_ShouldReturnError_WhenCalendarNotFound(int calendarId)
    {
        var result = await calendarService.DeleteCalendar(calendarId);

        await Assert.That(result).IsEqualTo("There is no calendar with the provided Id.");
        await calendarRepository.DidNotReceiveWithAnyArgs().DeleteCalendarAsync(default!, default);
    }

    [Test]
    public async Task UpdateCalendar_ShouldReturnError_WhenIdIsNull()
    {
        var request = new EditCalendarRequest
        {
            Id = null,
            Name = "Updated",
            StartTime = "08:00",
            EndTime = "17:00",
        };

        var result = await calendarService.UpdateCalendar(request);

        await Assert.That(result).IsEqualTo("The Id must be specified when updating a calendar.");
        await calendarRepository.DidNotReceive().UpdateCalendarAsync(Arg.Any<Calendar>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task UpdateCalendar_ShouldReturnError_WhenCalendarDoesNotExist()
    {
        var request = new EditCalendarRequest
        {
            Id = 99,
            Name = "Updated",
            StartTime = "08:00",
            EndTime = "17:00",
        };

        var result = await calendarService.UpdateCalendar(request);

        await Assert.That(result).IsEqualTo("There is no calendar with the provided Id.");
        await calendarRepository.DidNotReceive().UpdateCalendarAsync(Arg.Any<Calendar>(), Arg.Any<CancellationToken>());
    }

    [Test]
    [Arguments("17:00", "08:00")]
    [Arguments("08:00", "08:00")]
    public async Task UpdateCalendar_ShouldReturnError_WhenStartTimeIsNotBeforeEndTime(string startTime, string endTime)
    {
        var request = new EditCalendarRequest
        {
            Id = 1,
            Name = "Updated",
            StartTime = startTime,
            EndTime = endTime,
        };

        var result = await calendarService.UpdateCalendar(request);

        await Assert.That(result).IsEqualTo("The start time must be earlier than the end time.");
        await calendarRepository.DidNotReceive().UpdateCalendarAsync(Arg.Any<Calendar>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task UpdateCalendar_ShouldReturnError_WhenWorkHoursAreMalformed()
    {
        var request = new EditCalendarRequest
        {
            Id = 1,
            Name = "Updated",
            StartTime = "not a time",
            EndTime = "17:00",
        };

        var result = await calendarService.UpdateCalendar(request);

        await Assert.That(result).IsEqualTo("The format of the work hours is invalid. The correct format: HH:mm");
        await calendarRepository.DidNotReceive().UpdateCalendarAsync(Arg.Any<Calendar>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task UpdateCalendar_ShouldReturnError_WhenUpcomingAppointmentsFallOutsideNewHours()
    {
        //Booked tomorrow 08:00-09:00, but the owner is narrowing the calendar to start at 10:00.
        StubAppointmentsForCalendar(1, AppointmentAt(1, dayOffset: 1, "08:00", "09:00"));

        var request = new EditCalendarRequest
        {
            Id = 1,
            Name = "Updated",
            StartTime = "10:00",
            EndTime = "17:00",
        };

        var result = await calendarService.UpdateCalendar(request);

        await Assert
            .That(result)
            .IsEqualTo(
                "1 upcoming appointment(s) fall outside the new work hours. "
                    + "Move or cancel them before changing the hours."
            );
        await calendarRepository.DidNotReceive().UpdateCalendarAsync(Arg.Any<Calendar>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task UpdateCalendar_ShouldPass_WhenOnlyPastAppointmentsFallOutsideNewHours()
    {
        //Same 08:00 booking, but yesterday - history is not a reason to block the edit.
        StubAppointmentsForCalendar(1, AppointmentAt(1, dayOffset: -1, "08:00", "09:00"));

        var request = new EditCalendarRequest
        {
            Id = 1,
            Name = "Updated",
            StartTime = "10:00",
            EndTime = "17:00",
        };

        var result = await calendarService.UpdateCalendar(request);

        await Assert.That(result).IsNull();
        await calendarRepository.Received(1).UpdateCalendarAsync(Arg.Any<Calendar>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task UpdateCalendar_ShouldPass_WhenUpcomingAppointmentsFitInsideNewHours()
    {
        StubAppointmentsForCalendar(1, AppointmentAt(1, dayOffset: 1, "11:00", "12:00"));

        var request = new EditCalendarRequest
        {
            Id = 1,
            Name = "Updated",
            StartTime = "10:00",
            EndTime = "17:00",
        };

        var result = await calendarService.UpdateCalendar(request);

        await Assert.That(result).IsNull();
        await calendarRepository.Received(1).UpdateCalendarAsync(Arg.Any<Calendar>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task UpdateCalendar_ShouldPass_WhenValid()
    {
        var request = new EditCalendarRequest
        {
            Id = 1,
            Name = "Updated Calendar",
            StartTime = "07:00",
            EndTime = "19:00",
        };

        var result = await calendarService.UpdateCalendar(request);

        await Assert.That(result).IsNull();
        await calendarRepository
            .Received(1)
            .UpdateCalendarAsync(
                //OwnerId is not on the request, so the tracked entity must keep its original owner.
                Arg.Is<Calendar>(c =>
                    c.Id == 1
                    && c.Name == "Updated Calendar"
                    && c.StartTime == "07:00"
                    && c.EndTime == "19:00"
                    && c.OwnerId == "1"
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Test]
    public async Task GetCalendarById_ShouldReturnCalendar_WhenCalendarExists()
    {
        var result = await calendarService.GetCalendarById(1);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Name).IsEqualTo("Calendar 1");
        await Assert.That(result.StartTime).IsEqualTo("08:00");
    }

    [Test]
    public async Task GetCalendarById_ShouldReturnNull_WhenCalendarDoesNotExist()
    {
        var result = await calendarService.GetCalendarById(99);

        await Assert.That(result).IsNull();
    }

    private void SetUpRepository()
    {
        calendarRepository = Substitute.For<ICalendarRepository>();

        calendarRepository
            .GetCalendarsAsync(
                Arg.Any<Expression<Func<Calendar, bool>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo =>
            {
                var predicate = callInfo.ArgAt<Expression<Func<Calendar, bool>>>(0);

                var result = CalendarTestData.Calendars.AsEnumerable();

                if (predicate is not null)
                {
                    result = result.Where(predicate.Compile());
                }

                return result.ToList();
            });

        calendarRepository
            .GetCalendarByIdAsync(Arg.Any<int>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.ArgAt<int>(0);
                return CalendarTestData.Calendars.FirstOrDefault(x => x.Id == id);
            });

        calendarRepository
            .GetCalendarsForCustomerAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var customerId = callInfo.ArgAt<string>(0);
                if (customerId == "user-1")
                {
                    return CalendarTestData.Calendars.Take(1).ToList();
                }
                return [];
            });
    }

    //Default: no appointments on any calendar, so the work-hours conflict check is a no-op unless a
    //test opts in by re-stubbing this.
    private void SetUpAppointmentRepository()
    {
        appointmentRepository = Substitute.For<IAppointmentRepository>();

        appointmentRepository
            .GetAppointmentsForCalendarAsync(Arg.Any<int>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns([]);
    }

    private void StubAppointmentsForCalendar(int calendarId, params Appointment[] appointments)
    {
        appointmentRepository
            .GetAppointmentsForCalendarAsync(calendarId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(appointments.ToList());
    }

    //Builds an appointment on a fixed wall-clock time relative to today, so the assertions do not
    //depend on the hour the test happens to run at.
    private static Appointment AppointmentAt(int calendarId, int dayOffset, string startTime, string endTime)
    {
        var day = DateTime.Now.Date.AddDays(dayOffset);

        return new Appointment
        {
            Id = 1,
            CalendarId = calendarId,
            ServiceId = 1,
            UserId = "3",
            StartTime = day.Add(TimeOnly.Parse(startTime).ToTimeSpan()),
            EndTime = day.Add(TimeOnly.Parse(endTime).ToTimeSpan()),
            IsReadonly = false,
        };
    }

    private void SetUpUserManager()
    {
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        userManager = Substitute.For<UserManager<ApplicationUser>>(
            store,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );

        userManager
            .FindByEmailAsync("test@booker.com")
            .Returns(UserTestData.Users.First(x => x.Email == "test@booker.com"));
        userManager.FindByEmailAsync("notfound@booker.com").Returns((ApplicationUser?)null);
    }

    private IMapper SetUpMapper()
    {
        var mapper = Substitute.For<IMapper>();

        mapper
            .When(x => x.Map(Arg.Any<EditCalendarRequest>(), Arg.Any<Calendar>()))
            .Do(callInfo =>
            {
                var source = callInfo.ArgAt<EditCalendarRequest>(0);
                var destination = callInfo.ArgAt<Calendar>(1);

                destination.Name = source.Name;
                destination.StartTime = source.StartTime;
                destination.EndTime = source.EndTime;
            });

        mapper
            .Map<CalendarDto>(Arg.Any<Calendar>())
            .Returns(callInfo =>
            {
                var entity = callInfo.ArgAt<Calendar>(0);

                return new CalendarDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    StartTime = entity.StartTime,
                    EndTime = entity.EndTime,
                };
            });

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

        mapper
            .Map<List<UserDto>>(Arg.Any<List<ApplicationUser>>())
            .Returns(callInfo =>
            {
                var entities = callInfo.ArgAt<List<ApplicationUser>>(0);
                var dtos = new List<UserDto>();

                foreach (var entity in entities)
                {
                    dtos.Add(
                        new UserDto
                        {
                            Email = entity.Email!,
                            FirstName = entity.FirstName,
                            LastName = entity.LastName,
                        }
                    );
                }

                return dtos;
            });

        return mapper;
    }
}
