namespace Booker.Repository.Repositories;

public class InfoRepository(AppDbContext context) : IInfoRepository
{
    public string GetCurrency()
    {
        return context.Infos.AsNoTracking().First(x => x.Key == "Currency").Value;
    }
}
