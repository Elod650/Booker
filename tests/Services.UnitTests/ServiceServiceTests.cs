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
    public async Task AddService_ShouldPass_WhenIdIsNull()
    {
        var newService = new EditServiceRequest
        {
            CalendarId = 1,
            Name = "New Service",
            Duration = "00:45",
            Price = 150,
        };

        var result = await serviceService.AddService(newService);

        await Assert.That(result).IsNull();
        await serviceRepository
            .Received(1)
            .AddServiceAsync(
                Arg.Is<Service>(s =>
                    s.Name == "New Service"
                    && s.CalendarId == 1
                    && s.Price == 150
                    && s.Duration == TimeSpan.FromMinutes(45)
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Test]
    [Arguments(0)]
    [Arguments(1)]
    [Arguments(-1)]
    public async Task AddService_ShouldReturnError_WhenIdIsNotNull(int id)
    {
        var newService = CreateRequest(id);

        var result = await serviceService.AddService(newService);

        await Assert.That(result).IsEqualTo("The Id has to be null when adding a new service.");
        await serviceRepository.DidNotReceiveWithAnyArgs().AddServiceAsync(default!, default);
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    public async Task UpdateService_ShouldPass_WhenIdIsValid(int id)
    {
        var serviceToUpdate = new EditServiceRequest
        {
            Id = id,
            CalendarId = 1,
            Name = "Updated Service",
            Duration = "01:15",
            Price = 250,
        };

        var result = await serviceService.UpdateService(serviceToUpdate);

        await Assert.That(result).IsNull();
        await serviceRepository
            .Received(1)
            .UpdateServiceAsync(
                Arg.Is<Service>(s =>
                    s.Id == id
                    && s.Name == "Updated Service"
                    && s.Price == 250
                    && s.Duration == TimeSpan.FromMinutes(75)
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Test]
    public async Task UpdateService_ShouldReturnError_WhenIdIsNull()
    {
        var serviceToUpdate = CreateRequest(null);

        var result = await serviceService.UpdateService(serviceToUpdate);

        await Assert.That(result).IsEqualTo("The Id must be specified when updating a service.");
        await serviceRepository.DidNotReceiveWithAnyArgs().UpdateServiceAsync(default!, default);
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(int.MaxValue)]
    public async Task UpdateService_ShouldReturnError_WhenServiceNotFound(int id)
    {
        var serviceToUpdate = CreateRequest(id);

        var result = await serviceService.UpdateService(serviceToUpdate);

        await Assert.That(result).IsEqualTo("There is no service with the provided Id.");
        await serviceRepository.DidNotReceiveWithAnyArgs().UpdateServiceAsync(default!, default);
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    public async Task DeleteService_ShouldDeleteService_WhenIdIsValid(int id)
    {
        var result = await serviceService.DeleteService(id);

        await Assert.That(result).IsNull();
        await serviceRepository.Received(1).DeleteServiceAsync(Arg.Is<Service>(s => s.Id == id));
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(int.MaxValue)]
    public async Task DeleteService_ShouldReturnErrorMessage_WhenThereIsNoServiceWithId(int id)
    {
        var result = await serviceService.DeleteService(id);

        await Assert.That(result).IsEqualTo("There is no service with the provided Id.");
        await serviceRepository.DidNotReceiveWithAnyArgs().DeleteServiceAsync(default!, default);
    }

    private static EditServiceRequest CreateRequest(int? id) =>
        new()
        {
            Id = id,
            CalendarId = 1,
            Name = "Service",
            Duration = "00:30",
            Price = 100,
        };

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
            .Map<Service>(Arg.Any<EditServiceRequest>())
            .Returns(callInfo =>
            {
                var request = callInfo.ArgAt<EditServiceRequest>(0);

                return new Service
                {
                    Name = request.Name!,
                    CalendarId = request.CalendarId,
                    Duration = TimeSpan.Parse(request.Duration!),
                    Price = request.Price!.Value,
                };
            });

        mapper
            .When(x => x.Map(Arg.Any<EditServiceRequest>(), Arg.Any<Service>()))
            .Do(callInfo =>
            {
                var source = callInfo.ArgAt<EditServiceRequest>(0);
                var destination = callInfo.ArgAt<Service>(1);

                destination.Name = source.Name!;
                destination.CalendarId = source.CalendarId;
                destination.Duration = TimeSpan.Parse(source.Duration!);
                destination.Price = source.Price!.Value;
            });

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
