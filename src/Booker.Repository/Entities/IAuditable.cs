namespace Booker.Repository.Entities;

/// <summary>
/// Marks an entity whose audit timestamps are maintained automatically by <see cref="AppDbContext"/>.
/// </summary>
public interface IAuditable
{
    DateTime CreatedAt { get; set; }

    DateTime ModifiedAt { get; set; }
}
