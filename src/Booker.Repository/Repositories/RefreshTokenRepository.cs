namespace Booker.Repository.Repositories;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public async Task AddRefreshTokenAsync(RefreshToken newRefreshToken, CancellationToken cancellationToken = default)
    {
        await context.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default
    )
    {
        return await context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
    }

    public async Task RotateRefreshTokenAsync(
        RefreshToken oldToken,
        RefreshToken newToken,
        CancellationToken cancellationToken = default
    )
    {
        context.RefreshTokens.Update(oldToken);
        await context.RefreshTokens.AddAsync(newToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var tokens = await context
            .RefreshTokens.Where(x => x.SessionId == sessionId && x.RevokedAt == null)
            .ToListAsync(cancellationToken);

        await RevokeTokensAsync(tokens, cancellationToken);
    }

    public async Task DeleteExpiredTokensForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.UtcNow;

        var tokens = await context
            .RefreshTokens.Where(x => x.UserId == userId && (x.ExpiresAt <= now || x.RevokedAt != null))
            .ToListAsync(cancellationToken);

        if (tokens.Count == 0)
        {
            return;
        }

        context.RefreshTokens.RemoveRange(tokens);
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task RevokeTokensAsync(List<RefreshToken> tokens, CancellationToken cancellationToken)
    {
        if (tokens.Count == 0)
        {
            return;
        }

        DateTime now = DateTime.UtcNow;

        foreach (var token in tokens)
        {
            token.RevokedAt = now;
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
