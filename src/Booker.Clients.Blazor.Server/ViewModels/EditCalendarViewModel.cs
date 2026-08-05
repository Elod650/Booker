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
