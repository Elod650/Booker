namespace Booker.Backend;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<Appointment, AppointmentDto>().ReverseMap();
        CreateMap<Calendar, CalendarDto>();
        CreateMap<Service, ServiceDto>();
        CreateMap<EditServiceRequest, Service>();
    }
}
