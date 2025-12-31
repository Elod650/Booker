namespace Booker.Repository.Entities;

public class Appointment : EntityBase
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsReadonly { get; set; }

    public int ServiceId { get; set; }
    public int CalendarId { get; set; }
}
