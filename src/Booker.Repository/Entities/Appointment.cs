namespace Booker.Repository.Entities;

public class Appointment : EntityBase, IAuditable
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsReadonly { get; set; }
    public int ServiceId { get; set; }
    public int CalendarId { get; set; }
    public string UserId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public virtual Service? Service { get; set; }
    public virtual Calendar? Calendar { get; set; }
    public virtual ApplicationUser? User { get; set; }
}
