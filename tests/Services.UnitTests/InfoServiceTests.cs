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
    public async Task GetCurrency_ShouldReturnExactCurrencyValue_WhenInfoExists()
    {
        var result = await infoService.GetCurrency();

        await Assert.That(result).IsEqualTo("FT");
    }

    [Test]
    public async Task GetCurrency_ShouldThrowException_WhenInfoIsMissing()
    {
        infoRepository.GetInfoAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsNull();

        await Assert.ThrowsAsync(infoService.GetCurrency());
    }

    [Test]
    public async Task GetCurrency_ShouldReturnEmptyString_WhenCurrencyValueIsEmpty()
    {
        infoRepository
            .GetInfoAsync("Currency", Arg.Any<CancellationToken>())
            .Returns(new Info { Key = "Currency", Value = string.Empty });

        var result = await infoService.GetCurrency();

        await Assert.That(result).IsEmpty();
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
