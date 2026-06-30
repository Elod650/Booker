namespace Services.UnitTests;

public class ServiceServiceTests
{
    private ServiceService serviceService = null!;
    private IServiceRepository serviceRepository = null!;
    private ICalendarRepository calendarRepository = null!;

    [Before(Test)]
    public void SetUp()
    {
        SetUpRepository();
        var mapper = SetUpMapper();

        serviceService = new ServiceService(serviceRepository, calendarRepository, mapper);
    }

    [Test]
    [Arguments("1")]
    [Arguments("2")]
    public async Task GetServicesForUser_ShouldReturnList_WhenOwnerExists(string userId)
    {
        var result = await serviceService.GetServicesForUser(userId);

        await Assert.That(result.Count).IsEqualTo(1);
        await Assert.That(result[0].Name).IsEqualTo($"Service {userId}");
    }

    [Test]
    [Arguments("0")]
    public async Task GetServicesForUser_ShouldReturnEmptyList_WhenOwnerDoesNotExists(string userId)
    {
        var result = await serviceService.GetServicesForUser(userId);

        await Assert.That(result.Count).IsEqualTo(0);
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    public async Task GetServicesForCalendar_ShouldReturnDTOs(int calendarId)
    {
        var result = await serviceService.GetServicesForCalendar(calendarId);

        await Assert.That(result.Count).IsEqualTo(1);
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(int.MaxValue)]
    public async Task GetServicesForCalendar_ShouldReturnEmptyList_WhenCalendarIdIsInvalid(int calendarId)
    {
        var result = await serviceService.GetServicesForCalendar(calendarId);

        await Assert.That(result.Count).IsEqualTo(0);
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    public async Task GetServiceById_ShouldReturnDTO(int serviceId)
    {
        var result = await serviceService.GetServiceById(serviceId);

        await Assert.That(result).IsNotNull();
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(int.MaxValue)]
    public async Task GetServiceById_ShouldReturnNull_WhenIdIsINvalid(int serviceId)
    {
        var result = await serviceService.GetServiceById(serviceId);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task AddService_ShouldPass_WhenIdIsZero()
    {
        var newService = Substitute.For<EditServiceRequest>();

        var result = await serviceService.AddService(newService);

        await Assert.That(result).IsNull();
        await serviceRepository.Received(1).AddServiceAsync(Arg.Any<Service>());
    }

    [Test]
    [Arguments(1)]
    [Arguments(-1)]
    public async Task AddService_ShouldReturnError_WhenIdIsNotZero(int id)
    {
        var newService = Substitute.For<EditServiceRequest>();
        newService.Id = id;

        var result = await serviceService.AddService(newService);

        await Assert.That(result).EqualTo("The Id has to be 0 when adding a new service.");
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    public async Task UpdateService_ShouldPass_WhenIdIsValid(int id)
    {
        var serviceToUpdate = Substitute.For<EditServiceRequest>();
        serviceToUpdate.Id = id;

        var result = await serviceService.UpdateService(serviceToUpdate);

        await Assert.That(result).IsNull();
        await serviceRepository.Received(1).UpdateServiceAsync(Arg.Any<Service>());
    }

    [Test]
    [Arguments(1)]
    [Arguments(-1)]
    public async Task UpdateService_ShouldReturnError_WhenIdIsNotZero(int id)
    {
        var newService = Substitute.For<EditServiceRequest>();
        newService.Id = id;

        var result = await serviceService.AddService(newService);

        await Assert.That(result).EqualTo("The Id has to be 0 when adding a new service.");
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    public async Task DeleteServiceAsync_ShouldDeleteService_WhenIdIsValid(int id)
    {
        await serviceService.DeleteService(id);

        await serviceRepository.Received(1).DeleteServiceAsync(Arg.Is<Service>(s => s.Id == id));
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(int.MaxValue)]
    public async Task DeleteAppointmentAsync_ShouldReturnErrorMessage_WhenThereIsNoServiceWithId(int id)
    {
        var result = await serviceService.DeleteService(id);

        await Assert.That(result).IsEqualTo("There is no service with the provided Id.");
    }

    private void SetUpRepository()
    {
        serviceRepository = Substitute.For<IServiceRepository>();

        serviceRepository
            .GetServicesAsync(Arg.Any<Expression<Func<Service, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var predicate = callInfo.ArgAt<Expression<Func<Service, bool>>>(0);

                var result = ServiceTestData.Services.AsEnumerable();

                if (predicate is not null)
                {
                    result = result.Where(predicate.Compile());
                }

                return result.ToList();
            });

        serviceRepository
            .GetServiceByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.ArgAt<int>(0);
                return ServiceTestData.Services.FirstOrDefault(a => a.Id == id);
            });

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
            .GetCalendarIdsAsync(Arg.Any<Expression<Func<Calendar, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var predicate = callInfo.ArgAt<Expression<Func<Calendar, bool>>>(0);

                var result = CalendarTestData.Calendars.AsEnumerable();

                if (predicate is not null)
                {
                    result = result.Where(predicate.Compile());
                }

                return result.Select(x => x.Id).ToList();
            });
    }

    private IMapper SetUpMapper()
    {
        var mapper = Substitute.For<IMapper>();

        mapper
            .Map<List<ServiceDto>>(Arg.Any<List<Service>>())
            .Returns(callInfo =>
            {
                var entities = callInfo.ArgAt<List<Service>>(0);
                var dtos = new List<ServiceDto>();

                foreach (var entity in entities)
                {
                    dtos.Add(
                        new ServiceDto
                        {
                            Id = entity.Id,
                            Name = entity.Name,
                            Duration = entity.Duration,
                            Price = entity.Price,
                            CalendarId = entity.CalendarId,
                        }
                    );
                }

                return dtos;
            });

        mapper
            .Map<ServiceDto>(Arg.Any<Service>())
            .Returns(callInfo =>
            {
                var entity = callInfo.ArgAt<Service>(0);

                if (entity is null)
                {
                    return null;
                }

                var dto = new ServiceDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Duration = entity.Duration,
                    Price = entity.Price,
                    CalendarId = entity.CalendarId,
                };

                return dto;
            });

        return mapper;
    }
}
