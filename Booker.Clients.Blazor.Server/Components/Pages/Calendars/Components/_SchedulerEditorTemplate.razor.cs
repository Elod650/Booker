namespace Booker.Clients.Blazor.Server.Components.Pages.Calendars.Components;

public partial class _SchedulerEditorTemplate
{
    [Parameter, EditorRequired]
    public AppointmentDto Appointment { get; set; }

    [Parameter, EditorRequired]
    public List<ServiceDto> Services { get; set; }

    private void onServiceChange(int value)
    {
        Appointment.ServiceId = value;
        var selectedServiceDuration = Services.First(x => x.Id == Appointment.ServiceId).Duration;
        Appointment.EndTime = Appointment.StartTime?.Add(selectedServiceDuration);
    }

    protected override void OnParametersSet()
    {
        Appointment.ServiceId = Services.FirstOrDefault()?.Id ?? 0;
        onServiceChange(Appointment.ServiceId);
    }
}
