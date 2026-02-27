using Booker.Models.DTOs;

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
    public async Task AddAppointment_ShouldPass()
    {
        var newAppointment = Substitute.For<AppointmentDto>();

        await appointmentService.AddAppointment(newAppointment);

        await appointmentRepository.Received(1).AddAppointmentAsync(Arg.Any<Appointment>());
    }

    private void SetUpRepository()
    {
        appointmentRepository = Substitute.For<IAppointmentRepository>();

        appointmentRepository
            .GetAppointmentsForCalendarAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var calendarId = callInfo.ArgAt<int>(0);
                return Task.FromResult(
                    AppointmentTestData.Appointments.Where(a => a.CalendarId == calendarId).ToList()
                );
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
