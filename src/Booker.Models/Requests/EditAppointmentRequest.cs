namespace Booker.Models.Requests;

public class EditAppointmentRequest
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Start time is required")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "End time is required")]
    public DateTime EndTime { get; set; }

    public bool IsReadonly { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Service is required")]
    public int ServiceId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Calendar is required")]
    public int CalendarId { get; set; }
}
