namespace Booker.Services.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return null;
        }

        bool isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);
        string accessToken = GenerateAccessToken(user, roles);
        string refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);
        await userManager.UpdateAsync(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
        };
    }

    public async Task<string?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return "A user with this email already exists.";
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return string.Join("; ", result.Errors.Select(e => e.Description));
        }

        await userManager.AddToRoleAsync(user, RolesEnum.Customer.ToString());

        return null;
    }

    public async Task<AuthResponse?> RefreshTokenAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userManager.Users.FirstOrDefaultAsync(
            u => u.RefreshToken == request.RefreshToken,
            cancellationToken
        );

        if (user is null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);
        string accessToken = GenerateAccessToken(user, roles);
        string refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);
        await userManager.UpdateAsync(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
        };
    }

    private string GenerateAccessToken(ApplicationUser user, IList<string> roles)
    {
        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
        ];

        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        byte[] randomBytes = new byte[64];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }
}
