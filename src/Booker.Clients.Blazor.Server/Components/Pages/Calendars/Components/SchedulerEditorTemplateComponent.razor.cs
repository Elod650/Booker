namespace Booker.Clients.Blazor.Server.Components.Pages.Calendars.Components;

public partial class SchedulerEditorTemplateComponent
{
    [Parameter, EditorRequired]
    public AppointmentDto Appointment { get; set; }

    [Parameter, EditorRequired]
    public List<ServiceDto> Services { get; set; }

    private void OnServiceChange(int value)
    {
        try
        {
            Appointment.ServiceId = value;
            var selectedServiceDuration = Services.First(x => x.Id == Appointment.ServiceId).Duration;
            Appointment.EndTime = Appointment.StartTime?.Add(selectedServiceDuration);
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                $"An error occurred in {nameof(SchedulerEditorTemplateComponent)} during {nameof(OnServiceChange)}"
            );
        }
    }

    protected override void OnParametersSet()
    {
        try
        {
            Appointment.ServiceId = Services.FirstOrDefault()?.Id ?? 0;
            OnServiceChange(Appointment.ServiceId);
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                $"An error occurred in {nameof(SchedulerEditorTemplateComponent)} during {nameof(OnParametersSet)}"
            );
        }
    }
}
