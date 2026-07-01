namespace Booker.Repository.Entities;

[Index(nameof(RefreshToken))]
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<CalendarsXCustomers> CalendarsXCustomers { get; set; }
}
