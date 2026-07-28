namespace Booker.Repository.Configurations;

internal class CalendarsXCustomersConfiguration : IEntityTypeConfiguration<CalendarsXCustomers>
{
    public void Configure(EntityTypeBuilder<CalendarsXCustomers> builder)
    {
        builder.HasKey(cc => new { cc.CustomerId, cc.CalendarId });

        builder.Property(cc => cc.CustomerId).IsRequired();

        builder
            .HasOne(cc => cc.Customer)
            .WithMany(u => u.CalendarsXCustomers)
            .HasForeignKey(cc => cc.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(cc => cc.Calendar)
            .WithMany(c => c.CalendarsXCustomers)
            .HasForeignKey(cc => cc.CalendarId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
