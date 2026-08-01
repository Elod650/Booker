namespace Booker.Repository.Configurations;

internal class AppointmentConfiguration : EntityBaseConfiguration<Appointment>
{
    public override void Configure(EntityTypeBuilder<Appointment> builder)
    {
        base.Configure(builder);

        builder.Property(a => a.UserId).IsRequired();

        builder
            .HasOne(a => a.Service)
            .WithMany(s => s.Appointments)
            .HasForeignKey(a => a.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(a => a.Calendar)
            .WithMany(c => c.Appointments)
            .HasForeignKey(a => a.CalendarId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId);
    }
}
