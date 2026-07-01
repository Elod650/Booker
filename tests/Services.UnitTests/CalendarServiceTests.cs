namespace Services.UnitTests;

public class CalendarServiceTests
{
    private CalendarService calendarService = null!;
    private ICalendarRepository calendarRepository = null!;
    private UserManager<ApplicationUser> userManager = null!;

    [Before(Test)]
    public void SetUp()
    {
        SetUpRepository();
        SetUpUserManager();
        var mapper = SetUpMapper();

        calendarService = new CalendarService(calendarRepository, userManager, mapper);
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
            .GetCalendarsAsync(Arg.Any<Expression<Func<Calendar, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([]);

        var result = await calendarService.GetCalendars();

        await Assert.That(result).IsEmpty();
    }

    [Test]
    public async Task AddCalendar_ShouldPass_WhenIdIsZero()
    {
        var newCalendar = Substitute.For<EditCalendarRequest>();

        var result = await calendarService.AddCalendar(newCalendar, string.Empty);

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

        var result = await calendarService.AddCalendar(newCalendar, string.Empty);

        await Assert.That(result).EqualTo("The Id has to be 0 when adding a new calendar.");
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
        calendarRepository.GetCustomersForCalendarAsync(1, Arg.Any<CancellationToken>()).Returns([user]);

        var result = await calendarService.AddCustomerToCalendar(request);

        await Assert.That(result).EqualTo("The user is already added to the clanedar.");
    }

    [Test]
    public async Task AddCustomerToCalendar_ShouldPass_WhenCustomerAddedSuccessfully()
    {
        var request = new AddCustomerToCalendarRequest { CustomerEmail = "test@booker.com", CalendarId = 1 };

        calendarRepository.GetCustomersForCalendarAsync(1, Arg.Any<CancellationToken>()).Returns([]);

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
        calendarRepository.GetCustomersForCalendarAsync(1, Arg.Any<CancellationToken>()).Returns([user]);

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

    private void SetUpRepository()
    {
        calendarRepository = Substitute.For<ICalendarRepository>();

        calendarRepository
            .GetCalendarsAsync(Arg.Any<Expression<Func<Calendar, bool>>>(), Arg.Any<CancellationToken>())
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
            .GetCalendarByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.ArgAt<int>(0);
                return CalendarTestData.Calendars.FirstOrDefault(x => x.Id == id);
            });

        calendarRepository
            .GetCalendarsForCustomerAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
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
