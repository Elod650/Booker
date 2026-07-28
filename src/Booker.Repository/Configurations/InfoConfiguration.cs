namespace Booker.Repository.Configurations;

internal class InfoConfiguration : IEntityTypeConfiguration<Info>
{
    public void Configure(EntityTypeBuilder<Info> builder)
    {
        builder.HasKey(i => i.Key);

        builder.Property(i => i.Key).IsRequired();
        builder.Property(i => i.Value).IsRequired();
    }
}
