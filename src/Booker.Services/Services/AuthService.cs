namespace Booker.Services.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IRefreshTokenRepository refreshTokenRepository,
    IOptions<JwtOptions> jwtOptions
) : IAuthService
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

        await refreshTokenRepository.DeleteExpiredTokensForUserAsync(user.Id, cancellationToken);

        await refreshTokenRepository.AddRefreshTokenAsync(
            new RefreshToken
            {
                TokenHash = TokenHasher.ComputeHash(refreshToken),
                UserId = user.Id,
                SessionId = Guid.NewGuid(),
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
            },
            cancellationToken
        );

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
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return null;
        }

        var storedToken = await refreshTokenRepository.GetRefreshTokenByHashAsync(
            TokenHasher.ComputeHash(request.RefreshToken),
            asNoTracking: false,
            cancellationToken
        );

        if (storedToken is null)
        {
            return null;
        }

        //Reuse detection: a revoked token means it was already rotated (or explicitly revoked),
        //so presenting it again means the token was replayed and the session is compromised.
        //Revoke only that session - revoking every session would let anyone holding one stale
        //token log the user out everywhere.
        if (storedToken.RevokedAt is not null)
        {
            await refreshTokenRepository.RevokeSessionAsync(storedToken.SessionId, cancellationToken);

            return null;
        }

        if (storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        var user = await userManager.FindByIdAsync(storedToken.UserId);

        if (user is null)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);
        string accessToken = GenerateAccessToken(user, roles);
        string refreshToken = GenerateRefreshToken();
        string newTokenHash = TokenHasher.ComputeHash(refreshToken);

        storedToken.RevokedAt = DateTime.UtcNow;

        var newToken = new RefreshToken
        {
            TokenHash = newTokenHash,
            UserId = storedToken.UserId,
            SessionId = storedToken.SessionId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
        };

        await refreshTokenRepository.RotateRefreshTokenAsync(storedToken, newToken, cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
        };
    }

    /// <summary>
    /// Revokes the session the given refresh token belongs to. Idempotent - an unknown token is
    /// not an error, so the forced-logout path can safely replay an already-invalid token.
    /// </summary>
    public async Task LogoutAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return;
        }

        var storedToken = await refreshTokenRepository.GetRefreshTokenByHashAsync(
            TokenHasher.ComputeHash(request.RefreshToken),
            asNoTracking: false,
            cancellationToken
        );

        if (storedToken is null)
        {
            return;
        }

        await refreshTokenRepository.RevokeSessionAsync(storedToken.SessionId, cancellationToken);
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
