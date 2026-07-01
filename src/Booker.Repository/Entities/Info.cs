namespace Booker.Repository.Entities;

public class Info
{
    [Key, Required]
    public required string Key { get; set; }

    [Required]
    public required string Value { get; set; }
}
