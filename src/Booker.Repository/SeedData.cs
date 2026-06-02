namespace Booker.Repository;

public static class SeedData
{
    public static readonly string[] Roles =
    [
        RolesEnum.Admin.ToString(),
        RolesEnum.Provider.ToString(),
        RolesEnum.Customer.ToString(),
    ];

    public static List<Service> Services =
    [
        new Service
        {
            Id = 1,
            CalendarId = 1,
            Name = "Service 1",
            Duration = new TimeSpan(1, 30, 0),
            Price = 120,
        },
        new Service
        {
            Id = 2,
            CalendarId = 2,
            Name = "Service 2",
            Duration = new TimeSpan(1, 0, 0),
            Price = 100,
        },
        new Service
        {
            Id = 3,
            CalendarId = 2,
            Name = "Service 3",
            Duration = new TimeSpan(0, 30, 0),
            Price = 80,
        },
    ];

    public static List<Calendar> Calendars =
    [
        new Calendar
        {
            Id = 1,
            Code = Guid.NewGuid(),
            Name = "Calendar 1",
            StartTime = "08:00",
            EndTime = "16:00",
            OwnerId = "1",
        },
        new Calendar
        {
            Id = 2,
            Code = Guid.NewGuid(),
            Name = "Calendar 2",
            StartTime = "10:00",
            EndTime = "18:00",
            OwnerId = "1",
        },
    ];

    public static List<Info> Infos = [new Info { Key = "Currency", Value = "Ft" }];

    private static List<(string email, string password, string firstName, string lastName, string role)> users =
    [
        new("admin@booker.com", "Admin123!", "Admin", "User", RolesEnum.Admin.ToString()),
        new("provider@booker.com", "Provider123!", "Provider", "User", RolesEnum.Provider.ToString()),
        new("customer@booker.com", "Customer123!", "Customer", "User", RolesEnum.Customer.ToString()),
    ];

    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (string role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        foreach (var user in users)
        {
            var adminUser = new ApplicationUser
            {
                UserName = user.email,
                Email = user.email,
                FirstName = user.firstName,
                LastName = user.lastName,
                EmailConfirmed = true,
            };

            await userManager.CreateAsync(adminUser, user.password);
            await userManager.AddToRoleAsync(adminUser, user.role);
        }
    }
}
