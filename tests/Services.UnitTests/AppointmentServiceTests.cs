namespace Services.UnitTests;

public class AppointmentServiceTests
{
    private AppointmentService appointmentService = null!;
    private IAppointmentRepository appointmentRepository = null!;

    [Before(Test)]
    public void SetUp()
    {
        SetUpRepository();
        var mapper = SetUpMapper();

        appointmentService = new AppointmentService(appointmentRepository, mapper);
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
        var newAppointment = Substitute.For<EditAppointmentRequest>();
        var userId = "test-user-id";

        await appointmentService.AddAppointment(newAppointment, userId);

        await appointmentRepository.Received(1).AddAppointmentAsync(Arg.Is<Appointment>(a => a.UserId == userId));
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

    private void SetUpRepository()
    {
        appointmentRepository = Substitute.For<IAppointmentRepository>();

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
