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
    public HttpMethod Method { get; set; }

    /// <summary>
    /// The destination URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// The body of the request if there is any.
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="method">The HTTP method of the request.</param>
    /// <param name="url">The destination URL.</param>
    public ApiRequest(HttpMethod method, string url)
    {
        Method = method;
        Url = url;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="method">The HTTP method of the request.</param>
    /// <param name="url">The destination URL.</param>
    /// <param name="data">The body of the request.</param>
    public ApiRequest(HttpMethod method, string url, object data)
    {
        Method = method;
        Url = url;
        Data = data;
    }
}
