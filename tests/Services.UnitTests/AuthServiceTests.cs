namespace Services.UnitTests;

public class AuthServiceTests
{
    private IAuthService authService = null!;
    private UserManager<ApplicationUser> userManager = null!;
    private IOptions<JwtOptions> jwtOptions = null!;

    [Before(Test)]
    public void SetUp()
    {
        SetUpUserManager();
        SetUpJwtOptions();

        authService = new AuthService(userManager, jwtOptions);
    }

    [Test]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        var request = new LoginRequest { Email = "test@booker.com", Password = "Test123!" };

        var result = await authService.LoginAsync(request);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.AccessToken).IsNotEmpty();
        await Assert.That(result.RefreshToken).IsNotEmpty();
    }

    [Test]
    public async Task LoginAsync_ShouldReturnNull_WhenUserNotFound()
    {
        var request = new LoginRequest { Email = "notfound@booker.com", Password = "Test123!" };

        var result = await authService.LoginAsync(request);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsInvalid()
    {
        var request = new LoginRequest { Email = "test@booker.com", Password = "WrongPassword!" };

        var result = await authService.LoginAsync(request);

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

        var result = await authService.RegisterAsync(request);

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

        var result = await authService.RegisterAsync(request);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!).Contains("already exists");
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
    {
        // First login to get a refresh token
        var loginRequest = new LoginRequest { Email = "test@booker.com", Password = "Test123!" };
        var loginResult = await authService.LoginAsync(loginRequest);
        string refreshToken = loginResult!.RefreshToken;

        var refreshRequest = new RefreshTokenRequest { RefreshToken = refreshToken };

        var result = await authService.RefreshTokenAsync(refreshRequest);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.AccessToken).IsNotEmpty();
        await Assert.That(result.RefreshToken).IsNotEmpty();
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenIsInvalid()
    {
        var request = new RefreshTokenRequest { RefreshToken = "invalid-refresh-token" };

        var result = await authService.RefreshTokenAsync(request);

        await Assert.That(result).IsNull();
    }

    private void SetUpUserManager()
    {
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        userManager = Substitute.For<UserManager<ApplicationUser>>(
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

        var testUser = UserTestData.Users.First();

        userManager.FindByEmailAsync("test@booker.com").Returns(testUser);
        userManager.FindByEmailAsync("notfound@booker.com").Returns((ApplicationUser?)null);
        userManager.FindByEmailAsync("new@booker.com").Returns((ApplicationUser?)null);

        userManager.CheckPasswordAsync(testUser, "Test123!").Returns(true);
        userManager.CheckPasswordAsync(testUser, "WrongPassword!").Returns(false);

        userManager.GetRolesAsync(testUser).Returns(new List<string> { RolesEnum.Customer.ToString() });

        userManager.UpdateAsync(Arg.Any<ApplicationUser>()).Returns(IdentityResult.Success);

        userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);

        userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);

        // Setup Users queryable for RefreshToken lookup
        userManager.Users.Returns(new TestAsyncEnumerable<ApplicationUser>([testUser]));
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

        jwtOptions = Options.Create(options);
    }
}
