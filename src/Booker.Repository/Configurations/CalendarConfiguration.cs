namespace Booker.Repository.Configurations;

internal class CalendarConfiguration : EntityBaseConfiguration<Calendar>
{
    public override void Configure(EntityTypeBuilder<Calendar> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.Name).IsRequired();
        builder.Property(c => c.StartTime).IsRequired();
        builder.Property(c => c.EndTime).IsRequired();
        builder.Property(c => c.OwnerId).IsRequired();

        builder.HasOne(c => c.Owner).WithMany().HasForeignKey(c => c.OwnerId);
    }
}
