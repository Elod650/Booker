namespace Booker.ApiCaller.CallsForControllers.Interfaces
{
    public interface IInfoApiCaller
    {
        Task<string> GetCurrency(CancellationToken cancellationToken = default);
    }
}
