namespace Booker.Backend;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(
                dest => dest.BookingUser,
                opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}")
            );
        CreateMap<EditAppointmentRequest, Appointment>();

        CreateMap<Calendar, CalendarDto>();
        CreateMap<EditCalendarRequest, Calendar>();

        CreateMap<Service, ServiceDto>();
        CreateMap<EditServiceRequest, Service>();
    }
}
