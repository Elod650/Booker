namespace Booker.Repository.Repositories;

public class AppointmentRepository(AppDbContext context) : IAppointmentRepository
{
    public async Task<List<Appointment>> GetAppointmentsForCalendarAsync(
        int calendarId,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .Appointments.Where(x => x.CalendarId == calendarId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Appointment?> GetAppointmentById(int id, CancellationToken cancellationToken = default)
    {
        return await context.Appointments.FindAsync(keyValues: [id], cancellationToken: cancellationToken);
    }

    public async Task AddAppointmentAsync(Appointment newAppointment, CancellationToken cancellationToken = default)
    {
        await context.Appointments.AddAsync(newAppointment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAppointmentAsync(
        Appointment appointmentToDelete,
        CancellationToken cancellationToken = default
    )
    {
        context.Appointments.Remove(appointmentToDelete);
        await context.SaveChangesAsync(cancellationToken);
    }
}
