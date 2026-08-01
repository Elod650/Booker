namespace Booker.Repository.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddRefreshTokenAsync(RefreshToken newRefreshToken, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetRefreshTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task RotateRefreshTokenAsync(
        RefreshToken oldToken,
        RefreshToken newToken,
        CancellationToken cancellationToken = default
    );
    Task RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task DeleteExpiredTokensForUserAsync(string userId, CancellationToken cancellationToken = default);
}
