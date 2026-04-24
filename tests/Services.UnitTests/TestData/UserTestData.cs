using System;
using System.Collections.Generic;
using System.Text;

namespace Services.UnitTests.TestData;

internal static class UserTestData
{
    internal static readonly List<ApplicationUser> Users =
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
    ];
}
