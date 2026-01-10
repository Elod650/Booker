namespace Booker.ApiCaller;

internal static class ApiCallerBase
{
    private const string APPLICATION_JSON = "application/json";
    private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    /// <summary>
    /// Sends the specified API request asynchronously and deserializes the response content to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to which the response content is deserialized. Must be a reference type.</typeparam>
    /// <param name="request">The API request containing the neceserry data to include in the request.</param>
    /// <returns>The task result contains the deserialized response object of type T, or null if the response content is empty.</returns>
    /// <exception cref="ApiCallerException">Thrown when the there was an error during the API call.</exception>
    internal static async Task<T> SendWithResponseAsync<T>(ApiRequest request)
        where T : class, new()
    {
        try
        {
            var responseContent = await SendWithResponseAsync(request);
            return JsonSerializer.Deserialize<T>(responseContent, jsonOptions) ?? new();
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
    internal static async Task<string> SendWithResponseAsync(ApiRequest request)
    {
        try
        {
            var responseMessage = await SendMessageAsync(request);
            return await responseMessage.Content.ReadAsStringAsync();
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
    internal static async Task SendAsync(ApiRequest request)
    {
        try
        {
            await SendMessageAsync(request);
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
    private static async Task<HttpResponseMessage> SendMessageAsync(ApiRequest request)
    {
        HttpClient _httpClient = new HttpClient();

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

        HttpResponseMessage responseMessage = await _httpClient.SendAsync(message);

        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new ApiCallerException(
                $"Status code: {(int)responseMessage.StatusCode}, Reason phrase: {responseMessage.ReasonPhrase}"
            );
        }

        return responseMessage;
    }
}
