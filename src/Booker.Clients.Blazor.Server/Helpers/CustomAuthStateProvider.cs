namespace Booker.Clients.Blazor.Server.Helpers;

public class CustomAuthStateProvider(IStorageManager storageManager)
    : AuthenticationStateProvider,
        ICustomAuthStateProvider
{
    private const string ACCESS_TOKEN = "accessToken";

    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = await GetUserPrincipal();
        var state = new AuthenticationState(principal);
        NotifyAuthenticationStateChanged(Task.FromResult(state));
        return state;
    }

    public async Task LoginAsync(AuthResponse authResponse)
    {
        await storageManager.SetAsync(ACCESS_TOKEN, authResponse.AccessToken);

        var principal = await GetUserPrincipal();
        var state = new AuthenticationState(principal);
        NotifyAuthenticationStateChanged(Task.FromResult(state));
    }

    private async Task<ClaimsPrincipal> GetUserPrincipal()
    {
        string? token = await storageManager.GetAsync<string>(ACCESS_TOKEN);

        if (string.IsNullOrWhiteSpace(token))
        {
            return this._anonymous;
        }

        var claims = ParseClaimsFromJwt(token);

        if (claims is null)
        {
            return this._anonymous;
        }

        var identity = new ClaimsIdentity(claims, "CustomAuthentication");
        var principal = new ClaimsPrincipal(identity);

        return principal;
    }

    private static IEnumerable<Claim>? ParseClaimsFromJwt(string jwt)
    {
        string[] parts = jwt.Split('.');

        if (parts.Length != 3)
        {
            return null;
        }

        string payload = parts[1];

        // Pad base64 string if needed
        switch (payload.Length % 4)
        {
            case 2:
                payload += "==";
                break;
            case 3:
                payload += "=";
                break;
        }

        byte[] jsonBytes = Convert.FromBase64String(payload);
        string json = Encoding.UTF8.GetString(jsonBytes);

        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

        if (keyValuePairs is null)
        {
            return null;
        }

        List<Claim> claims = [];

        foreach (var kvp in keyValuePairs)
        {
            if (kvp.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in kvp.Value.EnumerateArray())
                {
                    claims.Add(new Claim(kvp.Key, element.GetString() ?? string.Empty));
                }
            }
            else
            {
                claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
            }
        }

        return claims;
    }
}
