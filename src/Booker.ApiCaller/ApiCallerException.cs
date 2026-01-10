namespace Booker.ApiCaller;

/// <summary>
/// Represents errors that occur during API calls made by the Booker.ApiCaller.
/// </summary>
/// <remarks>
/// Use this exception to indicate failures specific to API invocation, such as network errors, invalid
/// responses, or protocol violations. The exception message should provide details about the nature of the API call
/// failure.
/// </remarks>
public class ApiCallerException : Exception
{
    public ApiCallerException(string errorMessage)
        : base(errorMessage) { }
}
