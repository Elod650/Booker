using NSubstitute.ReturnsExtensions;

namespace Services.UnitTests;

public class InfoServiceTests
{
    private InfoService infoService = null!;
    private IInfoRepository infoRepository = null!;

    [Before(Test)]
    public void SetUp()
    {
        SetUpRepository();

        infoService = new InfoService(infoRepository);
    }

    [Test]
    public async Task GetCurrency_ShouldReturnCurrency()
    {
        var result = await infoService.GetCurrency();

        await Assert.That(result.Count).IsEqualTo(2);
    }

    [Test]
    public async Task GetCurrency_ShouldThrowException_WhenInfoIsMissing()
    {
        infoRepository.GetInfoAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsNull();

        await Assert.ThrowsAsync(infoService.GetCurrency());
    }

    private void SetUpRepository()
    {
        infoRepository = Substitute.For<IInfoRepository>();

        infoRepository
            .GetInfoAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var key = callInfo.ArgAt<string>(0);
                return InfoTestData.Infos.First(x => x.Key == key);
            });
    }
}
