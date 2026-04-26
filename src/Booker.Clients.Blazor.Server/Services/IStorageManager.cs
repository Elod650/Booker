namespace Booker.Clients.Blazor.Server.Services;

/// <summary>
/// Interface for managing storage operations in the Blazor server application.
/// </summary>
public interface IStorageManager
{
    /// <summary>
    /// Store the <paramref name="value"/> with the given <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    Task SetAsync(string key, object value);

    /// <summary>
    /// Retrieve the value which stored with the given <paramref name="key"/>.
    /// </summary>
    /// <typeparam name="T">The type of the stored data.</typeparam>
    /// <param name="key">The key.</param>
    /// <returns>The stored value.</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Delete the value which stored with the given <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    Task DeleteAsync(string key);
}
