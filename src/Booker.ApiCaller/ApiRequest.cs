namespace Booker.ApiCaller;

/// <summary>
/// The class for collect necessary data for the HTTP request.
/// </summary>
public class ApiRequest
{
    /// <summary>
    /// The HTTP method of the request.
    /// </summary>
    /// <remarks>
    /// Eg.: GET, POST
    /// </remarks>
    public HttpMethod Method { get; private set; }

    /// <summary>
    /// The destination URL.
    /// </summary>
    public string Url { get; private set; }

    /// <summary>
    /// The body of the request if there is any.
    /// </summary>
    public object? Data { get; private set; }

    private ApiRequest(HttpMethod method, string url, object? data = null)
    {
        Method = method;
        Url = url;
        Data = data;
    }

    /// <summary>
    /// Creates a new API request configured to send a POST request to the specified URL with the provided data as the request body.
    /// </summary>
    /// <param name="url">The destination URL.</param>
    /// <param name="data">The body of the request.</param>
    /// <returns>An <see cref="ApiRequest"/> instance representing the configured POST request.</returns>
    public static ApiRequest CreatePost(string url, object data) => new ApiRequest(HttpMethod.Post, url, data);

    /// <summary>
    /// Creates a new API request configured to send a PUT request to the specified URL with the provided data as the request body.
    /// </summary>
    /// <param name="url">The destination URL.</param>
    /// <param name="data">The body of the request.</param>
    /// <returns>An <see cref="ApiRequest"/> instance representing the configured PUT request.</returns>
    public static ApiRequest CreatePut(string url, object data) => new ApiRequest(HttpMethod.Put, url, data);

    /// <summary>
    /// Creates a new API request configured to send a GET request to the specified URL.
    /// </summary>
    /// <param name="url">The destination URL.</param>
    /// <returns>An <see cref="ApiRequest"/> instance representing the configured GET request.</returns>
    public static ApiRequest CreateGet(string url) => new ApiRequest(HttpMethod.Get, url);

    /// <summary>
    /// Creates a new API request configured to send a DELETE request to the specified URL.
    /// </summary>
    /// <param name="url">The destination URL.</param>
    /// <returns>An <see cref="ApiRequest"/> instance representing the configured DELETE request.</returns>
    public static ApiRequest CreateDelete(string url) => new ApiRequest(HttpMethod.Delete, url);
}
