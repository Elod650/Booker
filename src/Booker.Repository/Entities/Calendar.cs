namespace Booker.Repository.Entities;

public class Calendar : EntityBase
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string StartTime { get; set; } = null!;

    [Required]
    public string EndTime { get; set; } = null!;

    [Required]
    public required string OwnerId { get; set; }

    public virtual ApplicationUser? Owner { get; set; }

    public virtual ICollection<CalendarsXCustomers>? CalendarsXCustomers { get; set; }
}
