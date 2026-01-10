namespace Booker.Repository.Repositories;

public class AppointmentRepository(AppDbContext context, IMapper mapper) : IAppointmentRepository
{
    public List<AppointmentDto> GetAppointments(int calendarId)
    {
        return mapper.Map<List<AppointmentDto>>(context.Appointments.Where(x => x.CalendarId == calendarId));
    }

    public void AddAppointment(AppointmentDto newAppointment)
    {
        context.Appointments.Add(mapper.Map<Appointment>(newAppointment));
        context.SaveChanges();
    }
}
