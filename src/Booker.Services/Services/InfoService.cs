namespace Booker.Services.Services;

public class InfoService(IInfoRepository infoRepository) : IInfoService
{
    public async Task<string> GetCurrency()
    {
        var currencyInfo = await infoRepository.GetInfoAsync("Currency");
        return currencyInfo.Value;
    }
}
