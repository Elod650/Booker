namespace Booker.Services.Services;

public class InfoService(IInfoRepository infoRepository) : IInfoService
{
    public async Task<string> GetCurrency(CancellationToken cancellationToken = default)
    {
        var currencyInfo = await infoRepository.GetInfoAsync("Currency", cancellationToken: cancellationToken);
        return currencyInfo.Value;
    }
}
