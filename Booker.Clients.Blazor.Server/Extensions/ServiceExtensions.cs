namespace Booker.Clients.Blazor.Server.Extensions;

internal static class ServiceExtensions
{
    /// <summary>
    /// Configures application services and registers required dependencies.
    /// </summary>
    /// <param name="services">The service collection to which application services are added.</param>
    internal static void ConfigureServices(this IServiceCollection services)
    {
        services.AddRazorComponents().AddInteractiveServerComponents();

        services.AddSyncfusionBlazor();

        services.ConfigureApiCallers();
    }

    private static void ConfigureApiCallers(this IServiceCollection services)
    {
        services
            .AddScoped<IAppointmentApiCaller, AppointmentApiCaller>()
            .AddScoped<IServiceApiCaller, ServiceApiCaller>()
            .AddScoped<IInfoApiCaller, InfoApiCaller>()
            .AddScoped<ICalendarApiCaller, CalendarApiCaller>();
    }
}
