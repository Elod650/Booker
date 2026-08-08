namespace Booker.Clients.Blazor.Server.ViewModels;

public class EditCalendarViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = null!;

    [Required]
    public TimeOnly? StartTime { get; set; } = null!;

    [Required]
    public TimeOnly? EndTime { get; set; } = null!;

    public const string WorkHoursOrderErrorMessage = "The start time must be earlier than the end time.";

    public static EditCalendarViewModel Create(CalendarDto calendar)
    {
        return new EditCalendarViewModel
        {
            Id = calendar.Id,
            Name = calendar.Name,
            StartTime = TimeOnly.ParseExact(calendar.StartTime, "HH:mm", CultureInfo.InvariantCulture),
            EndTime = TimeOnly.ParseExact(calendar.EndTime, "HH:mm", CultureInfo.InvariantCulture),
        };
    }

    //Cross-field rule the data annotations cannot express. The server enforces the same rule in
    //CalendarService; this only avoids a pointless round-trip.
    public bool IsWorkHoursOrderValid()
    {
        return StartTime is null || EndTime is null || StartTime < EndTime;
    }

    public EditCalendarRequest ToRequest()
    {
        return new EditCalendarRequest
        {
            Id = Id,
            Name = Name,
            StartTime = StartTime?.ToString("HH:mm", CultureInfo.InvariantCulture),
            EndTime = EndTime?.ToString("HH:mm", CultureInfo.InvariantCulture),
        };
    }
}
