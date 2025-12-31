namespace Booker.Repository;

internal static class Mapper
{
    internal static AppointmentDto Map(this Appointment source)
    {
        return new AppointmentDto
        {
            Id = source.Id,
            StartTime = source.StartTime,
            EndTime = source.EndTime,
            IsReadonly = source.IsReadonly,
            ServiceId = source.ServiceId,
            CalendarId = source.CalendarId,
        };
    }

    internal static Appointment Map(this AppointmentDto source)
    {
        return new Appointment
        {
            Id = source.Id,
            StartTime = source.StartTime,
            EndTime = source.EndTime,
            IsReadonly = source.IsReadonly,
            ServiceId = source.ServiceId,
            CalendarId = source.CalendarId,
        };
    }

    internal static ServiceDto Map(this Service source)
    {
        return new ServiceDto
        {
            Id = source.Id,
            Name = source.Name,
            Duration = source.Duration,
            Price = source.Price,
            CalendarId = source.CalendarId,
        };
    }

    internal static CalendarDto Map(this Calendar source)
    {
        return new CalendarDto { Id = source.Id, Name = source.Name };
    }

    internal static List<AppointmentDto> Map(this IEnumerable<Appointment> source)
    {
        List<AppointmentDto> result = new();
        foreach (var item in source)
        {
            result.Add(item.Map());
        }
        return result;
    }

    internal static List<ServiceDto> Map(this IEnumerable<Service> source)
    {
        List<ServiceDto> result = new();
        foreach (var item in source)
        {
            result.Add(item.Map());
        }
        return result;
    }

    internal static List<CalendarDto> Map(this IEnumerable<Calendar> source)
    {
        List<CalendarDto> result = new();
        foreach (var item in source)
        {
            result.Add(item.Map());
        }
        return result;
    }
}
