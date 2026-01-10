namespace Booker.Backend.Extensions;

internal static class ServiceExtensions
{
    /// <summary>
    /// Configures application services and registers required dependencies.
    /// </summary>
    /// <param name="services">The service collection to which application services are added.</param>
    internal static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.LicenseKey = configuration["LicenseKeys:LuckyPenny"];
            cfg.AddProfile<AutoMapperConfig>();
        });
    }
}
