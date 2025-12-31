namespace Booker.Repository.Repositories;

public class AppointmentRepository(AppDbContext context) : IAppointmentRepository
{
    public List<AppointmentDto> GetAppointments(int calendarId)
    {
        return context.Appointments.Where(x => x.CalendarId == calendarId).Map();
    }

    public void AddAppointment(AppointmentDto newAppointment)
    {
        context.Appointments.Add(newAppointment.Map());
        context.SaveChanges();
    }
}
