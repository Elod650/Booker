namespace Booker.Models.Requests;

public class RemoveCustomerFromCalendarRequest
{
    public required string CustomerEmail { get; set; }

    public int CalendarId { get; set; }
}
