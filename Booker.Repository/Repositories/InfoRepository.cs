namespace Booker.Repository.Repositories;

public class InfoRepository : IInfoRepository
{
    public string GetCurrency()
    {
        return Database.Currency;
    }
}
