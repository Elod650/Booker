using Microsoft.Extensions.Configuration;

namespace Booker.Clients.Blazor.Server.Extensions;

internal static class ServiceExtensions
{
    /// <summary>
    /// Configures application services and registers required dependencies.
    /// </summary>
    /// <param name="services">The service collection to which application services are added.</param>
    internal static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRazorComponents().AddInteractiveServerComponents();

        services.AddSyncfusionBlazor();

        services.AddSerilog((s, lc) => lc.ReadFrom.Configuration(configuration));

        services.ConfigureApiCallers();
        services.ConfigureAuth();
    }

    private static void ConfigureApiCallers(this IServiceCollection services)
    {
        services
            .AddScoped<IAppointmentApiCaller, AppointmentApiCaller>()
            .AddScoped<IServiceApiCaller, ServiceApiCaller>()
            .AddScoped<IInfoApiCaller, InfoApiCaller>()
            .AddScoped<ICalendarApiCaller, CalendarApiCaller>();
    }

    private static void ConfigureAuth(this IServiceCollection services)
    {
        services.AddAuthentication();
        services.AddCascadingAuthenticationState();
        services.AddScoped<IStorageManager, SessionStorageManager>();
        services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
        services.AddScoped<ICustomAuthStateProvider, CustomAuthStateProvider>();
    }
}
