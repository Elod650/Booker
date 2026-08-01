namespace Booker.Repository.Entities;

public class RefreshToken : EntityBase, IAuditable
{
    public string TokenHash { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public Guid SessionId { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public virtual ApplicationUser? User { get; set; }
}
