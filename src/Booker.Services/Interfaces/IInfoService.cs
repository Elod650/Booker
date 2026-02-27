namespace Booker.Services.Interfaces;

public interface IInfoService
{
    Task<string> GetCurrency(CancellationToken cancellationToken = default);
}
