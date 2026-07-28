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

        ConfigureAuditableProperties(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyTimestamps();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default
    )
    {
        ApplyTimestamps();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// Configures the properties of entities that implement the IAuditable interface.
    /// </summary>
    /// <param name="modelBuilder"></param>
    private static void ConfigureAuditableProperties(ModelBuilder modelBuilder)
    {
        var auditableTypes = modelBuilder
            .Model.GetEntityTypes()
            .Where(e => typeof(IAuditable).IsAssignableFrom(e.ClrType));

        foreach (var entityType in auditableTypes)
        {
            modelBuilder.Entity(entityType.ClrType).Property(nameof(IAuditable.CreatedAt)).IsRequired();
            modelBuilder.Entity(entityType.ClrType).Property(nameof(IAuditable.ModifiedAt)).IsRequired();
        }
    }

    private void ApplyTimestamps()
    {
        DateTime now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State is EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.ModifiedAt = now;
                continue;
            }

            if (entry.State is EntityState.Modified)
            {
                entry.Entity.ModifiedAt = now;
                entry.Property(e => e.CreatedAt).IsModified = false;
            }
        }
    }
}
