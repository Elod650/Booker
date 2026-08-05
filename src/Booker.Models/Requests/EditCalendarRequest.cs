namespace Booker.Models.Requests;

public class EditCalendarRequest
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Start time is required")]
    [RegularExpression(
        @"^([01]\d|2[0-3]):[0-5]\d$",
        ErrorMessage = "The format of the Start time is invalid. The correct format: HH:mm"
    )]
    public string? StartTime { get; set; }

    [Required(ErrorMessage = "End time is required")]
    [RegularExpression(
        @"^([01]\d|2[0-3]):[0-5]\d$",
        ErrorMessage = "The format of the End time is invalid. The correct format: HH:mm"
    )]
    public string? EndTime { get; set; }
}
