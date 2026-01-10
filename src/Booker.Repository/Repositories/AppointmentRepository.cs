namespace Booker.Repository.Repositories;

public class AppointmentRepository(AppDbContext context, IMapper mapper) : IAppointmentRepository
{
    public List<AppointmentDto> GetAppointments(int calendarId)
    {
        return mapper.Map<List<AppointmentDto>>(
            context.Appointments.Where(x => x.CalendarId == calendarId).AsNoTracking()
        );
    }

    public async Task AddAppointment(AppointmentDto newAppointment)
    {
        await context.Appointments.AddAsync(mapper.Map<Appointment>(newAppointment));
        await context.SaveChangesAsync();
    }
}
