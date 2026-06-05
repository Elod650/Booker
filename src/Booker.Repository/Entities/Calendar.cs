namespace Booker.Repository.Entities;

public class Calendar : EntityBase
{
    public Guid Code { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string StartTime { get; set; }

    [Required]
    public string EndTime { get; set; }

    [Required]
    public string OwnerId { get; set; } = null!;

    public virtual ApplicationUser? Owner { get; set; }

    public ICollection<CalendarsXCustomers> CalendarsXCustomers;
}
