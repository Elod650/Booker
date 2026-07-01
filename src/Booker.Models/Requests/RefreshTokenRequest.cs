namespace Booker.Models.Requests;

public class RefreshTokenRequest
{
    [Required]
    public required string RefreshToken { get; set; }
}
