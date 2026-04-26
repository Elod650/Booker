namespace Booker.ApiCaller;

public class ApiCallerBase : IApiCallerBase
{
    private const string ACCEPT = "accept";
    private const string APPLICATION_JSON = "application/json";
    private const string BEARER = "Bearer";

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };
    private static HttpClient httpClient = new();

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
    public async Task<string> SendWithResponseAsync(
        ApiRequest request,
        bool withBearer = true,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var responseMessage = await SendMessageAsync(request, withBearer, cancellationToken);
            return await responseMessage.Content.ReadAsStringAsync(cancellationToken);
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
        HttpRequestMessage message = new(request.Method, request.Url);

        message.Headers.Add("Accept", APPLICATION_JSON);

        if (request.Data is not null)
        {
            message.Content = new StringContent(
                JsonSerializer.Serialize(request.Data),
                Encoding.UTF8,
                APPLICATION_JSON
            );
        }

        if (withBearer)
        {
            var bearerToken = await getAccessToken();

            if (bearerToken is not null && !string.IsNullOrWhiteSpace(bearerToken)) { }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        var messageFactory = () => BuildHttpRequestMessage(request);

        HttpResponseMessage responseMessage = withBearer
            ? await SendWithBearerToken(httpClient, messageFactory, cancellationToken)
            : await httpClient.SendAsync(messageFactory(), cancellationToken);

        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new ApiCallerException(
                $"Status code: {(int)responseMessage.StatusCode}, Reason phrase: {responseMessage.ReasonPhrase}"
            );
        }

        return responseMessage;
    }

    /// <summary>
    /// Retrieve the bearer token and send a request with it.
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="httpRequestMessageFactory"></param>
    /// <param name="withBearer"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> SendWithBearerToken(
        HttpClient httpClient,
        Func<HttpRequestMessage> messageFactory,
        CancellationToken cancellationToken = default
    )
    {
        var accessToken = await getAccessToken();

        //If we have an access token add it to request header.
        if (accessToken is not null && !string.IsNullOrWhiteSpace(accessToken))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER, accessToken);
        }

        try
        {
            var response = await httpClient.SendAsync(messageFactory(), cancellationToken);

            //If the response code is unauthorized then try to get a new one with the refresh token, then try the request again.
            if (!response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshToken = await getRefreshToken();

                //If we have an access token add it to request header.
                if (refreshToken is not null && !string.IsNullOrWhiteSpace(refreshToken))
                {
                    await InvokeRefreshTokensEndpoint(httpClient, refreshToken, cancellationToken);
                    response = await httpClient.SendAsync(messageFactory(), cancellationToken);
                }
            }

            return response;
        }
        catch (HttpRequestException ex)
        {
            var refreshToken = await getRefreshToken();

            //If we have an access token add it to request header.
            if (refreshToken is not null && !string.IsNullOrWhiteSpace(refreshToken))
            {
                await InvokeRefreshTokensEndpoint(httpClient, refreshToken, cancellationToken);
                return await httpClient.SendAsync(messageFactory(), cancellationToken);
            }
            throw;
        }
    }

    /// <summary>
    /// Get refreshed tokens from the API.
    /// </summary>
    /// <param name="httpClient">The http client.</param>
    /// <param name="tokens">The tokens.</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task InvokeRefreshTokensEndpoint(
        HttpClient httpClient,
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
            return;
        }

        var responseContentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        if (responseContentStream is null)
        {
            throw new Exception("Refresh response is empty");
        }

        var tokens = JsonSerializer.Deserialize<AuthResponse>(responseContentStream, _jsonOptions) ?? new();

        //If get the new tokens update the clients tokens and set it in the http client header.
        if (tokens is not null)
        {
            await updateTokens(tokens.AccessToken, tokens.RefreshToken);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BEARER, tokens.AccessToken);
        }
    }

    /// <summary>
    /// Create a <see cref="HttpRequestMessage"/> based on a <see cref="ApiRequest"/>.
    /// </summary>
    /// <param name="request">The parameters of the message.</param>
    /// <returns>The message.</returns>
    private HttpRequestMessage BuildHttpRequestMessage(ApiRequest request)
    {
        HttpRequestMessage message = new HttpRequestMessage();
        message.Headers.Add(ACCEPT, APPLICATION_JSON);
        message.RequestUri = new Uri(request.Url);
        message.Method = request.Method;

        if (request.Data != null)
        {
            var data = new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, APPLICATION_JSON);
            message.Content = data;
        }

        return message;
    }
}
