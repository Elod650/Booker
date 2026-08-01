namespace Booker.Repository.Repositories;

public class InfoRepository(AppDbContext context) : IInfoRepository
{
    public async Task<Info> GetInfoAsync(
        string key,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        var query = asNoTracking ? context.Infos.AsNoTracking() : context.Infos;

        return await query.FirstAsync(x => x.Key == key, cancellationToken);
    }
}
