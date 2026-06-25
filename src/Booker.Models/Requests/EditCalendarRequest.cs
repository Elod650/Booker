namespace Booker.Models.Requests;

public class EditCalendarRequest
{
    public int Id { get; set; }
    public Guid Code { get; set; }
    public string Name { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
}
