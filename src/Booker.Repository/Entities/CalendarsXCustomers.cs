namespace Booker.Repository.Entities;

public class CalendarsXCustomers
{
    public required string CustomerId { get; set; }
    public int CalendarId { get; set; }

    public virtual ApplicationUser? Customer { get; set; }
    public virtual Calendar? Calendar { get; set; }
}
