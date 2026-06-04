namespace Services.UnitTests.TestData;

internal static class CalendarTestData
{
    internal static readonly List<Calendar> Calendars = new List<Calendar>
    {
        new Calendar
        {
            Id = 1,
            Code = Guid.NewGuid(),
            Name = "Calendar 1",
            StartTime = "08:00",
            EndTime = "17:00",
            OwnerId = "1",
        },
        new Calendar
        {
            Id = 2,
            Code = Guid.NewGuid(),
            Name = "Calendar 2",
            StartTime = "09:00",
            EndTime = "18:00",
            OwnerId = "2",
        },
    };
}
