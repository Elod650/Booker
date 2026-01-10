namespace Booker.Backend.Extensions;

internal static class ServiceExtensions
{
    /// <summary>
    /// Configures application services and registers required dependencies.
    /// </summary>
    /// <param name="services">The service collection to which application services are added.</param>
    internal static void ConfigureServices(this IServiceCollection services)
    {
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
