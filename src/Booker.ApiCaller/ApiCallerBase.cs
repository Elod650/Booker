namespace Booker.ApiCaller;

public class ApiCallerBase(HttpClient httpClient) : IApiCallerBase
{
    private const string ACCEPT = "accept";
    private const string APPLICATION_JSON = "application/json";
    private const string BEARER = "Bearer";

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    private string _refreshUrl;

    public delegate Task<string?> GetAccessToken();
    public delegate Task<string?> GetRefreshToken();
    public delegate Task UpdateTokens(string accessToken, string refreshToken);
    public delegate Task Logout();

    private GetAccessToken getAccessToken;
    private GetRefreshToken getRefreshToken;
    private UpdateTokens updateTokens;
    private Logout logout;

    public void SetBasicData(
        string refreshUrl,
        GetAccessToken getAccessToken,
        GetRefreshToken getRefreshToken,
        UpdateTokens updateTokens,
        Logout logout
    )
    {
        _refreshUrl = refreshUrl;
        this.getAccessToken = getAccessToken;
        this.getRefreshToken = getRefreshToken;
        this.updateTokens = updateTokens;
        this.logout = logout;
    }

    /// <summary>
    /// Sends the specified API request asynchronously and deserializes the response content to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to which the response content is deserialized. Must be a reference type.</typeparam>
    /// <param name="request">The API request containing the neceserry data to include in the request.</param>
    /// <returns>The task result contains the deserialized response object of type T, or null if the response content is empty.</returns>
    /// <exception cref="ApiCallerException">Thrown when the there was an error during the API call.</exception>
    public async Task<T> SendWithResponseAsync<T>(
        ApiRequest request,
        bool withBearer = true,
        CancellationToken cancellationToken = default
    )
        where T : class, new()
    {
        try
        {
            var responseMessage = await SendMessageAsync(request, withBearer, cancellationToken);
            var responseContentStream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            return JsonSerializer.Deserialize<T>(responseContentStream, _jsonOptions) ?? new();
        }
        catch (ApiCallerException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new ApiCallerException("An unhandled exception occured during the deserialization.");
        }
    }

    /// <summary>
    /// Sends the specified API request asynchronously and returns the response content as a string.
    /// </summary>
    /// <param name="request">The API request containing the neceserry data to include in the request.</param>
    /// <returns>The task result contains the respons content as string.</returns>
    /// <exception cref="ApiCallerException">Thrown when the there was an error during the API call.</exception>
    public async Task<string?> SendWithResponseAsync(
        ApiRequest request,
        bool withBearer = true,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var responseMessage = await SendMessageAsync(request, withBearer, cancellationToken);
            var content = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
            return string.IsNullOrEmpty(content) ? null : content;
        }
        catch (ApiCallerException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new ApiCallerException("An unhandled exception occured during the API call.");
        }
    }

    /// <summary>
    /// Sends the specified API request asynchronously.
    /// </summary>
    /// <param name="request">The API request containing the neceserry data to include in the request.</param>
    /// <exception cref="ApiCallerException">Thrown when the there was an error during the API call.</exception>
    public async Task SendAsync(
        ApiRequest request,
        bool withBearer = true,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await SendMessageAsync(request, withBearer, cancellationToken);
        }
        catch (ApiCallerException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new ApiCallerException("An unhandled exception occured during the API call.");
        }
    }

    /// <summary>
    /// Sends an HTTP request asynchronously using the specified API request and returns the HTTP response message.
    /// </summary>
    /// <param name="request">The API request containing the neceserry data to include in the request.</param>
    /// <exception cref="ApiCallerException">Thrown when the server responds with a non-OK status.</exception>
    private async Task<HttpResponseMessage> SendMessageAsync(
        ApiRequest request,
        bool withBearer,
        CancellationToken cancellationToken = default
    )
    {
        var messageFactory = (string? accessToken) => BuildHttpRequestMessage(request, accessToken);

        HttpResponseMessage responseMessage = withBearer
            ? await SendWithBearerToken(messageFactory, cancellationToken)
            : await httpClient.SendAsync(messageFactory(null), cancellationToken);

        if (!responseMessage.IsSuccessStatusCode)
        {
            var apiResponseContent = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

            throw new ApiCallerException(
                $"Status code: {(int)responseMessage.StatusCode}, Reason phrase: {responseMessage.ReasonPhrase}",
                apiResponseContent
            );
        }

        return responseMessage;
    }

    /// <summary>
    /// Retrieve the bearer token and send a request with it.
    /// </summary>
    /// <param name="messageFactory">Creates a fresh request message carrying the supplied access token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response message.</returns>
    private async Task<HttpResponseMessage> SendWithBearerToken(
        Func<string?, HttpRequestMessage> messageFactory,
        CancellationToken cancellationToken = default
    )
    {
        var accessToken = await getAccessToken();

        try
        {
            var response = await httpClient.SendAsync(messageFactory(accessToken), cancellationToken);

            //If the response code is unauthorized then try to get a new one with the refresh token, then try the request again.
            if (response.StatusCode is not HttpStatusCode.Unauthorized)
            {
                return response;
            }

            var refreshedToken = await RefreshAccessTokenAsync(cancellationToken);

            if (refreshedToken is null)
            {
                return response;
            }

            return await httpClient.SendAsync(messageFactory(refreshedToken), cancellationToken);
        }
        catch (HttpRequestException)
        {
            var refreshedToken = await RefreshAccessTokenAsync(cancellationToken);

            if (refreshedToken is null)
            {
                throw;
            }

            return await httpClient.SendAsync(messageFactory(refreshedToken), cancellationToken);
        }
    }

    /// <summary>
    /// Exchange the stored refresh token for a new access token.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The new access token, or null when no refresh could be performed.</returns>
    private async Task<string?> RefreshAccessTokenAsync(CancellationToken cancellationToken)
    {
        var refreshToken = await getRefreshToken();

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return null;
        }

        return await InvokeRefreshTokensEndpoint(refreshToken, cancellationToken);
    }

    /// <summary>
    /// Get refreshed tokens from the API.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The new access token, or null when the client was logged out.</returns>
    /// <exception cref="Exception"></exception>
    private async Task<string?> InvokeRefreshTokensEndpoint(
        string refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        HttpRequestMessage message = new()
        {
            RequestUri = new Uri(_refreshUrl),
            Method = HttpMethod.Post,
            Content = new StringContent(
                JsonSerializer.Serialize(new RefreshTokenRequest { RefreshToken = refreshToken }),
                Encoding.UTF8,
                APPLICATION_JSON
            ),
        };
        message.Headers.Add(ACCEPT, APPLICATION_JSON);

        var response = await httpClient.SendAsync(message, cancellationToken);

        //If the response is an error message then log the client out.
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await logout();
            return null;
        }

        var responseContentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        if (responseContentStream is null)
        {
            throw new Exception("Refresh response is empty");
        }

        var tokens = JsonSerializer.Deserialize<AuthResponse>(responseContentStream, _jsonOptions) ?? new();

        //If we got the new tokens update the clients tokens.
        await updateTokens(tokens.AccessToken, tokens.RefreshToken);

        return string.IsNullOrWhiteSpace(tokens.AccessToken) ? null : tokens.AccessToken;
    }

    /// <summary>
    /// Create a <see cref="HttpRequestMessage"/> based on a <see cref="ApiRequest"/>.
    /// </summary>
    /// <param name="request">The parameters of the message.</param>
    /// <param name="accessToken">The bearer token to attach to this single message, if any.</param>
    /// <returns>The message.</returns>
    private HttpRequestMessage BuildHttpRequestMessage(ApiRequest request, string? accessToken)
    {
        HttpRequestMessage message = new HttpRequestMessage();
        message.Headers.Add(ACCEPT, APPLICATION_JSON);
        message.RequestUri = new Uri(request.Url);
        message.Method = request.Method;

        //The token is set on the message itself so concurrent callers never share auth state through the HttpClient.
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            message.Headers.Authorization = new AuthenticationHeaderValue(BEARER, accessToken);
        }

        if (request.Data is not null)
        {
            var data = new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, APPLICATION_JSON);
            message.Content = data;
        }

        return message;
    }
}
