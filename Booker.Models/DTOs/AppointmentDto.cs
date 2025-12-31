namespace Booker.Models.DTOs;

public class AppointmentDto
{
    public int Id { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsReadonly { get; set; }

    public int ServiceId { get; set; }
    public int CalendarId { get; set; }
}
