namespace Services.UnitTests.TestData;

internal static class UserTestData
{
    internal static List<ApplicationUser> Users =>
        [
            new ApplicationUser
            {
                Id = "user-1",
                UserName = "test@booker.com",
                Email = "test@booker.com",
                FirstName = "Test",
                LastName = "User",
                EmailConfirmed = true,
            },
            new ApplicationUser
            {
                Id = "user-2",
                UserName = "other@booker.com",
                Email = "other@booker.com",
                FirstName = "Other",
                LastName = "User",
                EmailConfirmed = true,
            },
        ];
}
