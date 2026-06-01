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

    public string OwnerId { get; set; }

    public EditCalendarRequest ToRequest()
    {
        return new EditCalendarRequest
        {
            Id = this.Id,
            Name = this.Name,
            StartTime = this.StartTime.ToString(),
            EndTime = this.EndTime.ToString(),
            OwnerId = this.OwnerId,
        };
    }
}
