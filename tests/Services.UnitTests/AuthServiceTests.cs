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

    [Test]
    public async Task RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenIsExpired()
    {
        var loginRequest = new LoginRequest { Email = "test@booker.com", Password = "Test123!" };
        var loginResult = await authService.LoginAsync(loginRequest);
        string refreshToken = loginResult!.RefreshToken;

        var testUser = userManager.Users.First(u => u.RefreshToken == refreshToken);
        testUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(-5);

        var refreshRequest = new RefreshTokenRequest { RefreshToken = refreshToken };

        var result = await authService.RefreshTokenAsync(refreshRequest);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task RegisterAsync_ShouldReturnErrors_WhenRegistrationFails()
    {
        var request = new RegisterRequest
        {
            Email = "fail@booker.com",
            Password = "FailPassword!",
            ConfirmPassword = "FailPassword!",
            FirstName = "Fail",
            LastName = "User",
        };

        var errors = new[]
        {
            new IdentityError { Description = "Password too simple" },
            new IdentityError { Description = "Invalid username" },
        };
        userManager
            .CreateAsync(Arg.Is<ApplicationUser>(u => u.Email == "fail@booker.com"), "FailPassword!")
            .Returns(IdentityResult.Failed(errors));

        var result = await authService.RegisterAsync(request);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!).IsEqualTo("Password too simple; Invalid username");
    }

    [Test]
    public async Task LoginAsync_ShouldThrowNullReferenceException_WhenRequestIsNull()
    {
        var action = () => authService.LoginAsync(null!);

        await Assert.ThrowsAsync<NullReferenceException>(action);
    }

    [Test]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenUserHasMultipleRoles()
    {
        var multipleRolesUser = new ApplicationUser
        {
            Id = "user-multiple-roles",
            UserName = "multi@booker.com",
            Email = "multi@booker.com",
            FirstName = "Multi",
            LastName = "Role",
            EmailConfirmed = true,
        };

        userManager.FindByEmailAsync("multi@booker.com").Returns(multipleRolesUser);
        userManager.CheckPasswordAsync(multipleRolesUser, "Test123!").Returns(true);
        userManager.GetRolesAsync(multipleRolesUser).Returns(["Customer", "Admin"]);

        var request = new LoginRequest { Email = "multi@booker.com", Password = "Test123!" };

        var result = await authService.LoginAsync(request);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.AccessToken).IsNotEmpty();
        await Assert.That(result.RefreshToken).IsNotEmpty();
    }

    [Test]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenUserHasNoRoles()
    {
        var noRolesUser = new ApplicationUser
        {
            Id = "user-no-roles",
            UserName = "noroles@booker.com",
            Email = "noroles@booker.com",
            FirstName = "No",
            LastName = "Roles",
            EmailConfirmed = true,
        };

        userManager.FindByEmailAsync("noroles@booker.com").Returns(noRolesUser);
        userManager.CheckPasswordAsync(noRolesUser, "Test123!").Returns(true);
        userManager.GetRolesAsync(noRolesUser).Returns([]);

        var request = new LoginRequest { Email = "noroles@booker.com", Password = "Test123!" };

        var result = await authService.LoginAsync(request);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.AccessToken).IsNotEmpty();
        await Assert.That(result.RefreshToken).IsNotEmpty();
    }

    [Test]
    public async Task RegisterAsync_ShouldThrowNullReferenceException_WhenRequestIsNull()
    {
        var action = () => authService.RegisterAsync(null!);

        await Assert.ThrowsAsync<NullReferenceException>(action);
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldThrowException_WhenRequestIsNull()
    {
        var action = () => authService.RefreshTokenAsync(null!);

        await Assert.ThrowsAsync<System.Reflection.TargetInvocationException>(action);
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenIsNull()
    {
        var nullTokenUser = new ApplicationUser
        {
            Id = "user-null-token",
            UserName = "nulltoken@booker.com",
            Email = "nulltoken@booker.com",
            FirstName = "Null",
            LastName = "Token",
            EmailConfirmed = true,
            RefreshToken = null,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1),
        };

        userManager.Users.Returns(new TestAsyncEnumerable<ApplicationUser>([nullTokenUser]));

        var request = new RefreshTokenRequest { RefreshToken = null! };

        var result = await authService.RefreshTokenAsync(request);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenIsEmpty()
    {
        var emptyTokenUser = new ApplicationUser
        {
            Id = "user-empty-token",
            UserName = "emptytoken@booker.com",
            Email = "emptytoken@booker.com",
            FirstName = "Empty",
            LastName = "Token",
            EmailConfirmed = true,
            RefreshToken = string.Empty,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1),
        };

        userManager.Users.Returns(new TestAsyncEnumerable<ApplicationUser>([emptyTokenUser]));

        var request = new RefreshTokenRequest { RefreshToken = string.Empty };

        var result = await authService.RefreshTokenAsync(request);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenUserHasMultipleRoles()
    {
        var multipleRolesUser = new ApplicationUser
        {
            Id = "user-multiple-roles-refresh",
            UserName = "multi-refresh@booker.com",
            Email = "multi-refresh@booker.com",
            FirstName = "Multi",
            LastName = "Role",
            EmailConfirmed = true,
            RefreshToken = "multi-refresh-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1),
        };

        userManager.Users.Returns(new TestAsyncEnumerable<ApplicationUser>([multipleRolesUser]));
        userManager.GetRolesAsync(multipleRolesUser).Returns(["Customer", "Admin"]);

        var request = new RefreshTokenRequest { RefreshToken = "multi-refresh-token" };

        var result = await authService.RefreshTokenAsync(request);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.AccessToken).IsNotEmpty();
        await Assert.That(result.RefreshToken).IsNotEmpty();
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenUserHasNoRoles()
    {
        var noRolesUser = new ApplicationUser
        {
            Id = "user-no-roles-refresh",
            UserName = "noroles-refresh@booker.com",
            Email = "noroles-refresh@booker.com",
            FirstName = "No",
            LastName = "Roles",
            EmailConfirmed = true,
            RefreshToken = "noroles-refresh-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1),
        };

        userManager.Users.Returns(new TestAsyncEnumerable<ApplicationUser>([noRolesUser]));
        userManager.GetRolesAsync(noRolesUser).Returns([]);

        var request = new RefreshTokenRequest { RefreshToken = "noroles-refresh-token" };

        var result = await authService.RefreshTokenAsync(request);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.AccessToken).IsNotEmpty();
        await Assert.That(result.RefreshToken).IsNotEmpty();
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
