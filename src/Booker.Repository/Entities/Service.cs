namespace Booker.Repository.Entities;

public class Service : EntityBase, IAuditable
{
    public int CalendarId { get; set; }

    public required string Name { get; set; }
    public TimeSpan Duration { get; set; }
    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
