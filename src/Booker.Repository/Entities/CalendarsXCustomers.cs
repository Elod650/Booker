namespace Booker.Repository.Entities;

public class CalendarsXCustomers : IAuditable
{
    public required string CustomerId { get; set; }
    public int CalendarId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public virtual ApplicationUser? Customer { get; set; }
    public virtual Calendar? Calendar { get; set; }
}
