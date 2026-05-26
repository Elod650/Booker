namespace Booker.Clients.Blazor.Server.Components.Common;

public partial class ConfirmationModal
{
    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public string Title { get; set; } = "Confirm Delete";

    [Parameter]
    public string Message { get; set; } = "Are you sure you want to delete this item?";

    [Parameter]
    public string ConfirmButtonText { get; set; } = "Delete";

    [Parameter]
    public string CancelButtonText { get; set; } = "Cancel";

    [Parameter]
    public EventCallback OnConfirm { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    private async Task Confirm()
    {
        await this.OnConfirm.InvokeAsync();
    }

    private async Task Cancel()
    {
        await this.OnCancel.InvokeAsync();
    }
}
