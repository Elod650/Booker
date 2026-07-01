namespace Booker.Repository.Entities;

public class Service : EntityBase
{
    public int CalendarId { get; set; }

    [Required]
    public required string Name { get; set; }
    public TimeSpan Duration { get; set; }
    public decimal Price { get; set; }
}
