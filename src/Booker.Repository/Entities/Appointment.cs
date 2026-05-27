namespace Booker.Repository.Entities;

public class Appointment : EntityBase
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsReadonly { get; set; }

    public int ServiceId { get; set; }
    public int CalendarId { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    public virtual Service? Service { get; set; }
    public virtual Calendar? Calendar { get; set; }
    public virtual ApplicationUser? User { get; set; }
}
