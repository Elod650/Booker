namespace Services.UnitTests;

public class ServiceServiceTests
{
    private ServiceService serviceService = null!;
    private IServiceRepository serviceRepository = null!;

    [Before(Test)]
    public void SetUp()
    {
        SetUpRepository();
        var mapper = SetUpMapper();

        serviceService = new ServiceService(serviceRepository, mapper);
    }

    [Test]
    public async Task GetServices_ShouldReturnDTOs()
    {
        var result = await serviceService.GetServices();

        await Assert.That(result.Count).IsEqualTo(2);
    }

    [Test]
    public async Task AddAppointment_ShouldPass()
    {
        var newService = Substitute.For<EditServiceRequest>();

        await serviceService.AddService(newService);

        await serviceRepository.Received(1).AddService(Arg.Any<Service>());
    }

    private void SetUpRepository()
    {
        serviceRepository = Substitute.For<IServiceRepository>();

        serviceRepository.GetServicesAsync(Arg.Any<CancellationToken>()).Returns(ServiceTestData.Services);

        serviceRepository
            .GetServicesForCalendarAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var calendarId = callInfo.ArgAt<int>(0);
                return ServiceTestData.Services.Where(s => s.CalendarId == calendarId).ToList();
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

        return mapper;
    }
}
