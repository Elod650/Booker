namespace Booker.Repository.Configurations;

internal class RefreshTokenConfiguration : EntityBaseConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        //Base64 encoded SHA-256 is 44 characters; 64 leaves headroom.
        builder.Property(r => r.TokenHash).IsRequired().HasMaxLength(64);

        builder.Property(r => r.UserId).IsRequired();

        builder.HasIndex(r => r.TokenHash).IsUnique();

        builder.HasIndex(r => r.SessionId);

        builder.HasIndex(r => r.UserId);

        builder.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId);
    }
}
