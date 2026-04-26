namespace Services.UnitTests;

public class AuthServiceTests
{
    private IAuthService authService = null!;
    private UserManager<ApplicationUser> userManager = null!;
    private IOptions<JwtOptions> jwtOptions = null!;

    [Before(Test)]
    public void SetUp()
    {
        this.SetUpUserManager();
        this.SetUpJwtOptions();

        this.authService = new AuthService(this.userManager, this.jwtOptions);
    }

    [Test]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        var request = new LoginRequest { Email = "test@booker.com", Password = "Test123!" };

        var result = await this.authService.LoginAsync(request);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.AccessToken).IsNotEmpty();
        await Assert.That(result.RefreshToken).IsNotEmpty();
    }

    [Test]
    public async Task LoginAsync_ShouldReturnNull_WhenUserNotFound()
    {
        var request = new LoginRequest { Email = "notfound@booker.com", Password = "Test123!" };

        var result = await this.authService.LoginAsync(request);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsInvalid()
    {
        var request = new LoginRequest { Email = "test@booker.com", Password = "WrongPassword!" };

        var result = await this.authService.LoginAsync(request);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task RegisterAsync_ShouldReturnNull_WhenRegistrationSucceeds()
    {
        var request = new RegisterRequest
        {
            Email = "new@booker.com",
            Password = "New123!",
            ConfirmPassword = "New123!",
            FirstName = "New",
            LastName = "User",
        };

        var result = await this.authService.RegisterAsync(request);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task RegisterAsync_ShouldReturnError_WhenEmailAlreadyExists()
    {
        var request = new RegisterRequest
        {
            Email = "test@booker.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!",
            FirstName = "Test",
            LastName = "User",
        };

        var result = await this.authService.RegisterAsync(request);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!).Contains("already exists");
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
    {
        // First login to get a refresh token
        var loginRequest = new LoginRequest { Email = "test@booker.com", Password = "Test123!" };
        var loginResult = await this.authService.LoginAsync(loginRequest);
        string refreshToken = loginResult!.RefreshToken;

        var refreshRequest = new RefreshTokenRequest { RefreshToken = refreshToken };

        var result = await this.authService.RefreshTokenAsync(refreshRequest);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.AccessToken).IsNotEmpty();
        await Assert.That(result.RefreshToken).IsNotEmpty();
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenIsInvalid()
    {
        var request = new RefreshTokenRequest { RefreshToken = "invalid-refresh-token" };

        var result = await this.authService.RefreshTokenAsync(request);

        await Assert.That(result).IsNull();
    }

    private void SetUpUserManager()
    {
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        this.userManager = Substitute.For<UserManager<ApplicationUser>>(
            store,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );

        this.userManager.FindByEmailAsync("test@booker.com").Returns(UserTestData.Users.First());
        this.userManager.FindByEmailAsync("notfound@booker.com").Returns((ApplicationUser?)null);
        this.userManager.FindByEmailAsync("new@booker.com").Returns((ApplicationUser?)null);

        this.userManager.CheckPasswordAsync(UserTestData.Users.First(), "Test123!").Returns(true);
        this.userManager.CheckPasswordAsync(UserTestData.Users.First(), "WrongPassword!").Returns(false);

        this.userManager.GetRolesAsync(UserTestData.Users.First())
            .Returns(new List<string> { RolesEnum.Customer.ToString() });

        this.userManager.UpdateAsync(Arg.Any<ApplicationUser>()).Returns(IdentityResult.Success);

        this.userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);

        this.userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);

        // Setup Users queryable for RefreshToken lookup
        this.userManager.Users.Returns(new List<ApplicationUser> { UserTestData.Users.First() }.AsQueryable());
    }

    private void SetUpJwtOptions()
    {
        var options = new JwtOptions
        {
            SecretKey = "TestSecretKeyThatIsAtLeast32CharactersLong!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            AccessTokenExpirationMinutes = 30,
            RefreshTokenExpirationDays = 7,
        };

        this.jwtOptions = Options.Create(options);
    }
}
