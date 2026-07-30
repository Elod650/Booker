namespace Booker.Repository.Configurations;

internal class AppointmentConfiguration : EntityBaseConfiguration<Appointment>
{
    public override void Configure(EntityTypeBuilder<Appointment> builder)
    {
        base.Configure(builder);

        builder.Property(a => a.UserId).IsRequired();

        builder.HasOne(a => a.Service).WithMany().HasForeignKey(a => a.ServiceId);

        builder.HasOne(a => a.Calendar).WithMany().HasForeignKey(a => a.CalendarId);

        builder.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId);
    }
}
