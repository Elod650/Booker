namespace Booker.Repository.Entities;

public class Calendar : EntityBase
{
    public string Name { get; set; } = null!;

    public string StartTime { get; set; } = null!;

    public string EndTime { get; set; } = null!;

    public required string OwnerId { get; set; }

    public virtual ApplicationUser? Owner { get; set; }

    public virtual ICollection<CalendarsXCustomers>? CalendarsXCustomers { get; set; }
}
