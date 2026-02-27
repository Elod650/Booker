namespace Booker.Services.Services;

public class InfoService(IInfoRepository infoRepository) : IInfoService
{
    public async Task<string> GetCurrency()
    {
        return infoRepository.GetCurrency();
    }
}
