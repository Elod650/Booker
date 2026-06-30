namespace Services.UnitTests.TestData;

internal static class AppointmentTestData
{
    internal static List<Appointment> Appointments =>
        [
            new Appointment
            {
                Id = 1,
                CalendarId = 1,
                ServiceId = 1,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                IsReadonly = false,
            },
            new Appointment
            {
                Id = 2,
                CalendarId = 1,
                ServiceId = 1,
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(3),
                IsReadonly = false,
            },
            new Appointment
            {
                Id = 3,
                CalendarId = 2,
                ServiceId = 1,
                StartTime = DateTime.Now.AddHours(4),
                EndTime = DateTime.Now.AddHours(5),
                IsReadonly = false,
            },
        ];
}
