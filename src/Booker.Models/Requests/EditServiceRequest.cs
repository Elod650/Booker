namespace Booker.Models.Requests;

public class EditServiceRequest
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Calendar is required")]
    public int CalendarId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Duration is required")]
    [RegularExpression(
        @"^\d{1,3}:[0-5]\d$",
        ErrorMessage = "The format of the Duration is invalid. The correct format: hh:mm"
    )]
    public string? Duration { get; set; }

    [Required(ErrorMessage = "Price is required")]
    public decimal? Price { get; set; }
}
