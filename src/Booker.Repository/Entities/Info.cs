namespace Booker.Repository.Entities;

public class Info : IAuditable
{
    public required string Key { get; set; }
    public required string Value { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
