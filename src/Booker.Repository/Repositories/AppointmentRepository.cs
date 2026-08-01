namespace Booker.Repository.Repositories;

public class AppointmentRepository(AppDbContext context) : IAppointmentRepository
{
    public async Task<List<Appointment>> GetAppointmentsForCalendarAsync(
        int calendarId,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        var query = asNoTracking ? context.Appointments.AsNoTracking() : context.Appointments;

        return await query.Where(x => x.CalendarId == calendarId).Include(x => x.User).ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingAppointmentAsync(
        int calendarId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .Appointments.AsNoTracking()
            .AnyAsync(
                x => x.CalendarId == calendarId && x.StartTime < endTime && startTime < x.EndTime,
                cancellationToken
            );
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(
        int id,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        var query = asNoTracking ? context.Appointments.AsNoTracking() : context.Appointments;

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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
