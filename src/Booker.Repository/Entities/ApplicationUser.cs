namespace Booker.Repository.Entities;

public class ApplicationUser : IdentityUser, IAuditable
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public virtual ICollection<CalendarsXCustomers> CalendarsXCustomers { get; set; }
}
