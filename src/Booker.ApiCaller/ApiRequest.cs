namespace Booker.ApiCaller;

/// <summary>
/// The class for collect necessary data for the HTTP request.
/// </summary>
internal class ApiRequest
{
    /// <summary>
    /// The HTTP method of the request.
    /// </summary>
    /// <remarks>
    /// Eg.: GET, POST
    /// </remarks>
    internal HttpMethod Method { get; private set; }

    /// <summary>
    /// The destination URL.
    /// </summary>
    internal string Url { get; private set; }

    /// <summary>
    /// The body of the request if there is any.
    /// </summary>
    internal object? Data { get; private set; }

    /// <summary>
    /// Creates a new API request configured to send a POST request to the specified URL with the provided data as the request body.
    /// </summary>
    /// <param name="url">The destination URL.</param>
    /// <param name="data">The body of the request.</param>
    /// <returns>An <see cref="ApiRequest"/> instance representing the configured POST request.</returns>
    internal static ApiRequest CreatePost(string url, object data)
    {
        return new ApiRequest
        {
            Method = HttpMethod.Post,
            Url = url,
            Data = data,
        };
    }

    /// <summary>
    /// Creates a new API request configured to send a GET request to the specified URL.
    /// </summary>
    /// <param name="url">The destination URL.</param>
    /// <returns>An <see cref="ApiRequest"/> instance representing the configured GET request.</returns>
    internal static ApiRequest CreateGet(string url)
    {
        return new ApiRequest { Method = HttpMethod.Get, Url = url };
    }

    /// <summary>
    /// Creates a new API request configured to send a DELETE request to the specified URL.
    /// </summary>
    /// <param name="url">The destination URL.</param>
    /// <returns>An <see cref="ApiRequest"/> instance representing the configured DELETE request.</returns>
    internal static ApiRequest CreateDelete(string url)
    {
        return new ApiRequest { Method = HttpMethod.Delete, Url = url };
    }
}
