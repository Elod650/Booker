namespace Services.UnitTests.TestData;

internal static class AppointmentTestData
{
    //All appointments are booked by customer "3". Calendar 1 is owned by "1", calendar 2 by "2".
    internal static List<Appointment> Appointments =>
        [
            new Appointment
            {
                Id = 1,
                CalendarId = 1,
                ServiceId = 1,
                UserId = "3",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                IsReadonly = false,
            },
            new Appointment
            {
                Id = 2,
                CalendarId = 1,
                ServiceId = 1,
                UserId = "3",
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(3),
                IsReadonly = false,
            },
            new Appointment
            {
                Id = 3,
                CalendarId = 2,
                ServiceId = 1,
                UserId = "3",
                StartTime = DateTime.Now.AddHours(4),
                EndTime = DateTime.Now.AddHours(5),
                IsReadonly = false,
            },
        ];
}
