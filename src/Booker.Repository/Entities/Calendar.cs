namespace Booker.Repository.Entities;

public class Calendar : EntityBase, IAuditable
{
    public string Name { get; set; } = null!;

    public string StartTime { get; set; } = null!;

    public string EndTime { get; set; } = null!;

    public required string OwnerId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public virtual ApplicationUser? Owner { get; set; }

    public virtual ICollection<CalendarsXCustomers>? CalendarsXCustomers { get; set; }

    public virtual ICollection<Appointment>? Appointments { get; set; }

    public virtual ICollection<Service>? Services { get; set; }
}
