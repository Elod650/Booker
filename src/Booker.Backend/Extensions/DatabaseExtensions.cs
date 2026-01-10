namespace Booker.Backend.Extensions;

internal static class DatabaseExtensions
{
    /// <summary>
    /// Configures the application's database.
    /// </summary>
    /// <param name="services">The service collection to which the database be added.</param>
    internal static void ConfigureDatabase(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("BookerDb");

            options.UseSeeding(
                (context, _) =>
                {
                    context.AddRange(SeedData.Calendars);
                    context.AddRange(SeedData.Services);
                    context.AddRange(SeedData.Infos);
                    context.SaveChanges();
                }
            );
        });
    }
}
