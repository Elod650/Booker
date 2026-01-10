namespace Booker.ApiCaller;

internal static class ApiCallerBase
{
    private const string APPLICATION_JSON = "application/json";
    private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    private static HttpClient httpClient = new();

    /// <summary>
    /// Sends the specified API request asynchronously and deserializes the response content to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to which the response content is deserialized. Must be a reference type.</typeparam>
    /// <param name="request">The API request containing the neceserry data to include in the request.</param>
    /// <returns>The task result contains the deserialized response object of type T, or null if the response content is empty.</returns>
    /// <exception cref="ApiCallerException">Thrown when the there was an error during the API call.</exception>
    internal static async Task<T> SendWithResponseAsync<T>(
        ApiRequest request,
        CancellationToken cancellationToken = default
    )
        where T : class, new()
    {
        try
        {
            var responseMessage = await SendMessageAsync(request, cancellationToken);
            var responseContentStream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            return JsonSerializer.Deserialize<T>(responseContentStream, jsonOptions) ?? new();
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
    internal static async Task<string> SendWithResponseAsync(
        ApiRequest request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var responseMessage = await SendMessageAsync(request, cancellationToken);
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
    internal static async Task SendAsync(ApiRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await SendMessageAsync(request, cancellationToken);
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
    private static async Task<HttpResponseMessage> SendMessageAsync(
        ApiRequest request,
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

        HttpResponseMessage responseMessage = await httpClient.SendAsync(message, cancellationToken);

        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new ApiCallerException(
                $"Status code: {(int)responseMessage.StatusCode}, Reason phrase: {responseMessage.ReasonPhrase}"
            );
        }

        return responseMessage;
    }
}
