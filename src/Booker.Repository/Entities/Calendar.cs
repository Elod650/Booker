namespace Booker.Repository.Entities;

public class Calendar : EntityBase
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string StartTime { get; set; }

    [Required]
    public string EndTime { get; set; }

    [Required]
    public string OwnerId { get; set; } = null!;

    public virtual ApplicationUser? Owner { get; set; }

    public virtual ICollection<CalendarsXCustomers> CalendarsXCustomers { get; set; }
}
