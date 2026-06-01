namespace Booker.Clients.Blazor.Server.Components.Pages.Calendars.Components;

public partial class EditCalendarComponent
{
    [Parameter]
    public EditCalendarViewModel Model { get; set; } = new();

    [Parameter, EditorRequired]
    public EventCallback OnSaveEvent { get; set; }
}
