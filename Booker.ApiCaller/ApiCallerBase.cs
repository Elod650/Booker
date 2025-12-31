namespace Booker.ApiCaller;

internal static class ApiCallerBase
{
    private const string APPLICATION_JSON = "application/json";

    internal static async Task<T> SendAsync<T>(ApiRequest request)
        where T : class
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

        string? errorMessage = null;

        if (responseMessage.IsSuccessStatusCode)
        {
            switch (responseMessage.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    errorMessage = "The url is invalid";
                    break;
                case HttpStatusCode.Unauthorized:
                    errorMessage = "Unauthorized";
                    break;
                case HttpStatusCode.Forbidden:
                    errorMessage = "Forbidden";
                    break;
                default:
                    break;
            }

            if (errorMessage is not null)
            {
                throw new ApiCallerException(errorMessage);
            }
        }

        var responseContent = await responseMessage.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContent);
    }
}
