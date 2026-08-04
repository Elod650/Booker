namespace Services.UnitTests.TestData;

internal static class CalendarTestData
{
    internal static List<Calendar> Calendars =>
        [
            new Calendar
            {
                Id = 1,
                Name = "Calendar 1",
                StartTime = "08:00",
                EndTime = "17:00",
                OwnerId = "1",
            },
            new Calendar
            {
                Id = 2,
                Name = "Calendar 2",
                StartTime = "09:00",
                EndTime = "18:00",
                OwnerId = "2",
            },
        ];

    //Customer "3" is invited to calendar 1 only; customer "4" is invited to nothing.
    internal static List<CalendarsXCustomers> CalendarsXCustomers =>
        [new CalendarsXCustomers { CalendarId = 1, CustomerId = "3" }];
}
