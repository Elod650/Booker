namespace Booker.Repository;

public static class SeedData
{
    public static List<Service> Services = new()
    {
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
    };
    public static List<Calendar> Calendars = new()
    {
        new Calendar
        {
            Id = 1,
            Code = Guid.NewGuid(),
            Name = "Calendar 1",
        },
        new Calendar
        {
            Id = 2,
            Code = Guid.NewGuid(),
            Name = "Calendar 2",
        },
    };
    public static List<Info> Infos = new()
    {
        new Info { Key = "Currency", Value = "Ft" },
    };
}
