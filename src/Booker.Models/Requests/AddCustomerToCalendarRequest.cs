namespace Booker.Models.Requests;

public class AddCustomerToCalendarRequest
{
    public required string CustomerEmail { get; set; }

    public int CalendarId { get; set; }
}
