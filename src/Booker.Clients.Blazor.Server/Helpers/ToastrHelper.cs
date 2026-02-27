namespace Booker.Clients.Blazor.Server.Helpers;

internal static class ToastrHelper
{
    const string TOASTR_METHOD_NAME = "showToast";

    /// <summary>
    /// Show a success toast message with toastr.
    /// </summary>
    /// <param name="js">Blazor js runtime.</param>
    /// <param name="message">The messgae of the toast messaage.</param>
    internal static async Task SuccessToast(this IJSRuntime jsRuntime, string message)
    {
        await jsRuntime.InvokeVoidAsync(TOASTR_METHOD_NAME, "success", message);
    }

    /// <summary>
    /// Show an error toast message with toastr.
    /// </summary>
    /// <param name="js">Blazor js runtime.</param>
    /// <param name="message">The messgae of the toast messaage.</param>
    internal static async Task ErrorToast(this IJSRuntime jsRuntime, string message)
    {
        await jsRuntime.InvokeVoidAsync(TOASTR_METHOD_NAME, "error", message);
    }
}
