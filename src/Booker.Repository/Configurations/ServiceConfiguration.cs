namespace Booker.Repository.Configurations;

internal class ServiceConfiguration : EntityBaseConfiguration<Service>
{
    public override void Configure(EntityTypeBuilder<Service> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.Name).IsRequired();
    }
}
