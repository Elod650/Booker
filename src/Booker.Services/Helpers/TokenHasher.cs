namespace Booker.Services.Helpers;

/// <summary>
/// Hashes refresh tokens for storage. Refresh tokens are high-entropy random values, so an
/// unsalted SHA-256 is sufficient and keeps lookups a single indexed equality match.
/// </summary>
public static class TokenHasher
{
    public static string ComputeHash(string token)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
