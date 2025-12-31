namespace Booker.ApiCaller;

internal class ApiCallerException : Exception
{
    public ApiCallerException(string errorMessage)
        : base(errorMessage) { }
}
