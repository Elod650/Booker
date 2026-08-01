namespace Booker.Repository.Entities;

public class Service : EntityBase, IAuditable
{
    public int CalendarId { get; set; }

    public required string Name { get; set; }
    public TimeSpan Duration { get; set; }
    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public virtual Calendar? Calendar { get; set; }

    public virtual ICollection<Appointment>? Appointments { get; set; }
}
