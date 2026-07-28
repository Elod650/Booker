namespace Booker.Repository;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Calendar> Calendars { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Info> Infos { get; set; }
    public DbSet<CalendarsXCustomers> CalendarsXCustomers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<EntityBase>();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
