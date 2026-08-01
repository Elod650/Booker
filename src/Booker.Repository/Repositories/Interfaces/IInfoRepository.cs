namespace Booker.Repository.Repositories.Interfaces;

public interface IInfoRepository
{
    Task<Info> GetInfoAsync(string key, bool asNoTracking = true, CancellationToken cancellationToken = default);
}
