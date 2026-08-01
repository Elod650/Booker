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
        services.AddHttpClient(nameof(ApiCallerBase));

        services
            .AddScoped<IApiCallerBase>(serviceProvider => new ApiCallerBase(
                serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(ApiCallerBase))
            ))
            .AddScoped<IAppointmentApiCaller, AppointmentApiCaller>()
            .AddScoped<IAuthApiCaller, AuthApiCaller>()
            .AddScoped<IServiceApiCaller, ServiceApiCaller>()
            .AddScoped<IInfoApiCaller, InfoApiCaller>()
            .AddScoped<ICalendarApiCaller, CalendarApiCaller>();

        services.AddScoped<ApiCallerMediator>();
    }

    private static void ConfigureAuth(this IServiceCollection services)
    {
        services.AddAuthentication();
        services.AddCascadingAuthenticationState();
        services.AddScoped<IStorageManager, SessionStorageManager>();

        //One instance per circuit, with both handles forwarding to it. Registering the type twice
        //would create two independent instances, so state changes notified through one would not
        //reach the components subscribed to the other.
        services.AddScoped<CustomAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(serviceProvider =>
            serviceProvider.GetRequiredService<CustomAuthStateProvider>()
        );
        services.AddScoped<ICustomAuthStateProvider>(serviceProvider =>
            serviceProvider.GetRequiredService<CustomAuthStateProvider>()
        );
    }
}
