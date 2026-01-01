namespace Booker.Clients.Blazor.Server.Extensions;

internal static class OptionsExtensions
{
    internal static void ConfigureOptions(this IServiceCollection services)
    {
        services.AddOptions<ApiCallerOptions>().BindConfiguration(nameof(ApiCallerOptions));
    }
}
