Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    Log.Information("Starting API");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    builder.Services.ConfigureDatabase();
    builder.Services.ConfigureServices(builder.Configuration);
    builder.Services.ConfigureAuthentication(builder.Configuration);

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        //Need for seeding the database on startup, can be removed when using a real database with migrations
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();

            await SeedData.SeedRolesAndAdminAsync(scope.ServiceProvider);
            await SeedData.SeedCalendarsAsync(scope.ServiceProvider, context);
        }

        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.ConfigureMidlewares();

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
