namespace Booker.Repository.Repositories.Interfaces;

public interface IInfoRepository
{
    Task<Info> GetInfoAsync(string key, CancellationToken cancellationToken = default);
}
