using Booker.Repository.Repositories;

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
        var serviceToDelete = ServiceTestData.Services.First(a => a.Id == id);

        var result = await serviceService.DeleteService(id);

        await serviceRepository.Received(1).DeleteServiceAsync(serviceToDelete);
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

        serviceRepository.GetServicesAsync(Arg.Any<CancellationToken>()).Returns(ServiceTestData.Services);

        serviceRepository
            .GetServiceByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.ArgAt<int>(0);
                return ServiceTestData.Services.FirstOrDefault(a => a.Id == id);
            });

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
