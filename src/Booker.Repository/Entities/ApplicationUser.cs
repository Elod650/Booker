namespace Booker.Repository.Entities;

public class ApplicationUser : IdentityUser
{
    public required string FirstName { get; set; }

    public required string LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<CalendarsXCustomers> CalendarsXCustomers { get; set; }
}
