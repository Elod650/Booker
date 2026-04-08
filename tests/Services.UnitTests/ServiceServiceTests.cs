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
    public async Task AddAppointment_ShouldPass()
    {
        var newService = Substitute.For<EditServiceRequest>();

        await serviceService.AddService(newService);

        await serviceRepository.Received(1).AddService(Arg.Any<Service>());
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
            .GetServicesForCalendarAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var calendarId = callInfo.ArgAt<int>(0);
                return ServiceTestData.Services.Where(s => s.CalendarId == calendarId).ToList();
            });

        serviceRepository
            .GetServiceByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.ArgAt<int>(0);
                return ServiceTestData.Services.FirstOrDefault(a => a.Id == id);
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
