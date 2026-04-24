namespace Booker.Clients.Blazor.Server.Services;

public class SessionStorageManager(ProtectedSessionStorage sessionStorage) : IStorageManager
{
    public async Task SetAsync(string key, object value)
    {
        await sessionStorage.SetAsync(key, value);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var result = await sessionStorage.GetAsync<T>(key);
        return result.Success ? result.Value : default;
    }

    public async Task DeleteAsync(string key)
    {
        await sessionStorage.DeleteAsync(key);
    }
}
