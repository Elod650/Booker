namespace Booker.Repository.Entities;

public class Calendar : EntityBase
{
    public Guid Code { get; set; }
    public string Name { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
}
