namespace Booker.Repository.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    public List<AppointmentDto> GetAppointments(int calendarId)
    {
        return Database.Appointments.Where(x => x.CalendarId == calendarId).Map();
    }

    public void AddAppointment(AppointmentDto newAppointment)
    {
        newAppointment.Id = Database.Appointments.Any() ? Database.Appointments.Max(x => x.Id) + 1 : 1;
        Database.Appointments.Add(newAppointment.Map());
    }
}
