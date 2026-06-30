namespace Services.UnitTests.TestData;

internal static class ServiceTestData
{
    internal static List<Service> Services =>
        [
            new Service
            {
                Id = 1,
                Name = "Service 1",
                Duration = TimeSpan.FromMinutes(30),
                Price = 100,
                CalendarId = 1,
            },
            new Service
            {
                Id = 2,
                Name = "Service 2",
                Duration = TimeSpan.FromMinutes(60),
                Price = 200,
                CalendarId = 2,
            },
        ];
}
