namespace Booker.Repository.Entities;

[PrimaryKey(nameof(CustomerId), nameof(CalendarId))]
public class CalendarsXCustomers
{
    public string CustomerId { get; set; }
    public int CalendarId { get; set; }

    public virtual ApplicationUser? Customer { get; set; }
    public virtual Calendar? Calendar { get; set; }
}
