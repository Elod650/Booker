namespace Booker.Backend.Extensions;

internal static class MiddlewareExtensions
{
    /// <summary>
    /// Adds the middlewares to the application's request processing pipeline.
    /// </summary>
    internal static void ConfigureMidlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalErrorHandlingMiddleware>();
    }
}
