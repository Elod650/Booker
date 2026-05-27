namespace Booker.Clients.Blazor.Server.Components.Pages.Calendars.Components;

public partial class SchedulerEditorTemplateComponent
{
    [Parameter, EditorRequired]
    public SchedulerAppointmentViewModel Appointment { get; set; }

    [Parameter, EditorRequired]
    public List<ServiceDto> Services { get; set; }

    private async Task OnServiceChange(int value)
    {
        try
        {
            Appointment.ServiceId = value;
            var selectedServiceDuration = Services.First(x => x.Id == Appointment.ServiceId).Duration;
            Appointment.EndTime = Appointment.StartTime.Add(selectedServiceDuration);
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                $"An error occurred in {nameof(SchedulerEditorTemplateComponent)} during {nameof(OnServiceChange)}"
            );
            await JSRuntime.ErrorToast("An error occured during service change");
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        try
        {
            Appointment.ServiceId = Services.FirstOrDefault()?.Id ?? 0;
            await OnServiceChange(Appointment.ServiceId);
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                $"An error occurred in {nameof(SchedulerEditorTemplateComponent)} during {nameof(OnParametersSet)}"
            );
            await JSRuntime.ErrorToast("An error occured during the loading of the appointment editor");
        }
    }
}
