namespace Booker.Repository;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Calendar> Calendars { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Info> Infos { get; set; }
}
