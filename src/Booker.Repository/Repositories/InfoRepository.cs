namespace Booker.Repository.Repositories;

public class InfoRepository(AppDbContext context) : IInfoRepository
{
    public async Task<Info> GetInfoAsync(string key, CancellationToken cancellationToken = default)
    {
        return await context.Infos.AsNoTracking().FirstAsync(x => x.Key == key, cancellationToken);
    }
}
