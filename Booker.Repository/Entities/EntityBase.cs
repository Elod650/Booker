namespace Booker.Repository.Entities;

public abstract class EntityBase
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
}
