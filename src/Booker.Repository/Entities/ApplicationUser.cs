namespace Booker.Repository.Entities;

[Index(nameof(RefreshToken))]
public class ApplicationUser : IdentityUser
{
    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<CalendarsXCustomers> CalendarsXCustomers { get; set; }
}
