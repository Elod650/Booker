namespace Booker.Clients.Blazor.Server.Extensions;

internal static class ServiceExtensions
{
    /// <summary>
    /// Add services to the container.
    /// </summary>
    /// <param name="services"></param>
    internal static void ConfigureServices(this IServiceCollection services)
    {
        services.AddRazorComponents().AddInteractiveServerComponents();

        services.AddSyncfusionBlazor();

        services.ConfigureRepositories();
    }

    private static void ConfigureRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<IAppointmentRepository, AppointmentRepository>()
            .AddScoped<ICalendarRepository, CalendarRepository>()
            .AddScoped<IInfoRepository, InfoRepository>()
            .AddScoped<IServiceRepository, ServiceRepository>();
    }
}
