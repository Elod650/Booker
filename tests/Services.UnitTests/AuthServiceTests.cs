namespace Services.UnitTests;

public class AuthServiceTests
{
    private IAuthService authService = null!;
    private UserManager<ApplicationUser> userManager = null!;
    private IRefreshTokenRepository refreshTokenRepository = null!;
    private List<RefreshToken> tokens = null!;
    private IOptions<JwtOptions> jwtOptions = null!;
    private ApplicationUser testUser = null!;
    private ApplicationUser otherUser = null!;

    [Before(Test)]
    public void SetUp()
    {
        SetUpUserManager();
        SetUpJwtOptions();
        SetUpRefreshTokenRepository();

        authService = new AuthService(userManager, refreshTokenRepository, jwtOptions);
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
    public async Task LoginAsync_ShouldStoreHashedToken_AndNeverStoreRawToken()
    {
        var request = new LoginRequest { Email = "test@booker.com", Password = "Test123!" };

        var result = await authService.LoginAsync(request);

        var storedToken = tokens.Single();

        await Assert.That(storedToken.TokenHash).IsEqualTo(TokenHasher.ComputeHash(result!.RefreshToken));
        await Assert.That(storedToken.TokenHash).IsNotEqualTo(result.RefreshToken);
        await Assert.That(tokens.Any(x => x.TokenHash == result.RefreshToken)).IsFalse();
    }

    [Test]
    public async Task LoginAsync_ShouldCreateIndependentSessions_WhenCalledTwice()
    {
        var request = new LoginRequest { Email = "test@booker.com", Password = "Test123!" };

        var firstLogin = await authService.LoginAsync(request);
        var secondLogin = await authService.LoginAsync(request);

        var sessionIds = tokens.Select(x => x.SessionId).Distinct().ToList();

        await Assert.That(sessionIds.Count).IsEqualTo(2);

        //Both sessions must remain usable - logging in on a second device must not kill the first.
        var firstRefresh = await authService.RefreshTokenAsync(
            new RefreshTokenRequest { RefreshToken = firstLogin!.RefreshToken }
        );
        var secondRefresh = await authService.RefreshTokenAsync(
            new RefreshTokenRequest { RefreshToken = secondLogin!.RefreshToken }
        );

        await Assert.That(firstRefresh).IsNotNull();
        await Assert.That(secondRefresh).IsNotNull();
    }

    [Test]
    public async Task LoginAsync_ShouldDeleteExpiredAndRevokedTokensForUser()
    {
        tokens.Add(CreateToken(testUser.Id, "expired-hash", expiresAt: DateTime.UtcNow.AddDays(-1)));
        tokens.Add(CreateToken(testUser.Id, "revoked-hash", revokedAt: DateTime.UtcNow));
        tokens.Add(CreateToken(testUser.Id, "active-hash"));

        var request = new LoginRequest { Email = "test@booker.com", Password = "Test123!" };

        await authService.LoginAsync(request);

        await Assert.That(tokens.Any(x => x.TokenHash == "expired-hash")).IsFalse();
        await Assert.That(tokens.Any(x => x.TokenHash == "revoked-hash")).IsFalse();
        await Assert.That(tokens.Any(x => x.TokenHash == "active-hash")).IsTrue();
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
        await Assert.That(result.RefreshToken).IsNotEqualTo(refreshToken);
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldMarkOldTokenReplaced_AndKeepSameSessionId()
    {
        var loginRequest = new LoginRequest { Email = "test@booker.com", Password = "Test123!" };
        var loginResult = await authService.LoginAsync(loginRequest);

        var result = await authService.RefreshTokenAsync(
            new RefreshTokenRequest { RefreshToken = loginResult!.RefreshToken }
        );

        var oldToken = tokens.Single(x => x.TokenHash == TokenHasher.ComputeHash(loginResult.RefreshToken));
        var newToken = tokens.Single(x => x.TokenHash == TokenHasher.ComputeHash(result!.RefreshToken));

        await Assert.That(oldToken.RevokedAt).IsNotNull();
        await Assert.That(newToken.SessionId).IsEqualTo(oldToken.SessionId);
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

        var storedToken = tokens.First(x => x.TokenHash == TokenHasher.ComputeHash(refreshToken));
        storedToken.ExpiresAt = DateTime.UtcNow.AddMinutes(-5);

        var refreshRequest = new RefreshTokenRequest { RefreshToken = refreshToken };

        var result = await authService.RefreshTokenAsync(refreshRequest);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldReturnNull_WhenTokenIsRevoked()
    {
        var loginResult = await authService.LoginAsync(
            new LoginRequest { Email = "test@booker.com", Password = "Test123!" }
        );

        var storedToken = tokens.First(x => x.TokenHash == TokenHasher.ComputeHash(loginResult!.RefreshToken));
        storedToken.RevokedAt = DateTime.UtcNow;

        var result = await authService.RefreshTokenAsync(
            new RefreshTokenRequest { RefreshToken = loginResult!.RefreshToken }
        );

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldRevokeSession_WhenReplacedTokenIsReused()
    {
        var loginResult = await authService.LoginAsync(
            new LoginRequest { Email = "test@booker.com", Password = "Test123!" }
        );
        string originalToken = loginResult!.RefreshToken;

        var rotated = await authService.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = originalToken });

        //Replaying the original token must be treated as a compromise.
        var reuseResult = await authService.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = originalToken });

        await Assert.That(reuseResult).IsNull();

        //The legitimately rotated token must be dead too - the whole session is revoked.
        var afterReuse = await authService.RefreshTokenAsync(
            new RefreshTokenRequest { RefreshToken = rotated!.RefreshToken }
        );

        await Assert.That(afterReuse).IsNull();
        await Assert.That(tokens.All(x => x.RevokedAt is not null)).IsTrue();
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldNotRevokeOtherSessions_WhenReuseIsDetected()
    {
        var request = new LoginRequest { Email = "test@booker.com", Password = "Test123!" };

        var firstSession = await authService.LoginAsync(request);
        var secondSession = await authService.LoginAsync(request);

        await authService.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = firstSession!.RefreshToken });

        //Trip reuse detection on the first session only.
        await authService.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = firstSession.RefreshToken });

        var secondSessionResult = await authService.RefreshTokenAsync(
            new RefreshTokenRequest { RefreshToken = secondSession!.RefreshToken }
        );

        await Assert.That(secondSessionResult).IsNotNull();
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldReturnNull_WhenUserNoLongerExists()
    {
        var loginResult = await authService.LoginAsync(
            new LoginRequest { Email = "test@booker.com", Password = "Test123!" }
        );

        userManager.FindByIdAsync(testUser.Id).Returns((ApplicationUser?)null);

        var result = await authService.RefreshTokenAsync(
            new RefreshTokenRequest { RefreshToken = loginResult!.RefreshToken }
        );

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task LogoutAsync_ShouldRevokeOnlyCurrentSession()
    {
        var request = new LoginRequest { Email = "test@booker.com", Password = "Test123!" };

        var firstSession = await authService.LoginAsync(request);
        var secondSession = await authService.LoginAsync(request);

        await authService.LogoutAsync(new RefreshTokenRequest { RefreshToken = firstSession!.RefreshToken });

        var firstResult = await authService.RefreshTokenAsync(
            new RefreshTokenRequest { RefreshToken = firstSession.RefreshToken }
        );
        var secondResult = await authService.RefreshTokenAsync(
            new RefreshTokenRequest { RefreshToken = secondSession!.RefreshToken }
        );

        await Assert.That(firstResult).IsNull();
        await Assert.That(secondResult).IsNotNull();
    }

    [Test]
    public async Task LogoutAsync_ShouldNotThrow_WhenTokenIsUnknown()
    {
        await authService.LogoutAsync(new RefreshTokenRequest { RefreshToken = "unknown-token" });

        await Assert.That(tokens).IsEmpty();
    }

    [Test]
    public async Task LogoutAsync_ShouldNotThrow_WhenTokenIsNullOrEmpty()
    {
        await authService.LogoutAsync(new RefreshTokenRequest { RefreshToken = null! });
        await authService.LogoutAsync(new RefreshTokenRequest { RefreshToken = string.Empty });

        await Assert.That(tokens).IsEmpty();
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
    public async Task RefreshTokenAsync_ShouldThrowNullReferenceException_WhenRequestIsNull()
    {
        var action = () => authService.RefreshTokenAsync(null!);

        await Assert.ThrowsAsync<NullReferenceException>(action);
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenIsNull()
    {
        var request = new RefreshTokenRequest { RefreshToken = null! };

        var result = await authService.RefreshTokenAsync(request);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenIsEmpty()
    {
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
        };

        userManager.FindByIdAsync(multipleRolesUser.Id).Returns(multipleRolesUser);
        userManager.GetRolesAsync(multipleRolesUser).Returns(["Customer", "Admin"]);

        tokens.Add(CreateToken(multipleRolesUser.Id, TokenHasher.ComputeHash("multi-refresh-token")));

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
        };

        userManager.FindByIdAsync(noRolesUser.Id).Returns(noRolesUser);
        userManager.GetRolesAsync(noRolesUser).Returns([]);

        tokens.Add(CreateToken(noRolesUser.Id, TokenHasher.ComputeHash("noroles-refresh-token")));

        var request = new RefreshTokenRequest { RefreshToken = "noroles-refresh-token" };

        var result = await authService.RefreshTokenAsync(request);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.AccessToken).IsNotEmpty();
        await Assert.That(result.RefreshToken).IsNotEmpty();
    }

    private static RefreshToken CreateToken(
        string userId,
        string tokenHash,
        DateTime? expiresAt = null,
        DateTime? revokedAt = null
    )
    {
        return new RefreshToken
        {
            TokenHash = tokenHash,
            UserId = userId,
            SessionId = Guid.NewGuid(),
            ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(7),
            RevokedAt = revokedAt,
        };
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

        var users = UserTestData.Users;
        testUser = users.First(x => x.Id == "user-1");
        otherUser = users.First(x => x.Id == "user-2");

        userManager.FindByEmailAsync("test@booker.com").Returns(testUser);
        userManager.FindByEmailAsync("notfound@booker.com").Returns((ApplicationUser?)null);
        userManager.FindByEmailAsync("new@booker.com").Returns((ApplicationUser?)null);

        userManager.FindByIdAsync(testUser.Id).Returns(testUser);
        userManager.FindByIdAsync(otherUser.Id).Returns(otherUser);

        userManager.CheckPasswordAsync(testUser, "Test123!").Returns(true);
        userManager.CheckPasswordAsync(testUser, "WrongPassword!").Returns(false);

        userManager.GetRolesAsync(testUser).Returns(new List<string> { RolesEnum.Customer.ToString() });

        userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);

        userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);
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

    private void SetUpRefreshTokenRepository()
    {
        tokens = [];
        refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();

        refreshTokenRepository.AddRefreshTokenAsync(
            Arg.Do<RefreshToken>(newToken => tokens.Add(newToken)),
            Arg.Any<CancellationToken>()
        );

        refreshTokenRepository
            .GetRefreshTokenByHashAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var tokenHash = callInfo.ArgAt<string>(0);
                return tokens.FirstOrDefault(x => x.TokenHash == tokenHash);
            });

        //The old token instance is the same reference GetRefreshTokenByHashAsync handed back, so
        //its RevokedAt mutation is already reflected in the list - only the new row needs adding.
        refreshTokenRepository.RotateRefreshTokenAsync(
            Arg.Any<RefreshToken>(),
            Arg.Do<RefreshToken>(newToken => tokens.Add(newToken)),
            Arg.Any<CancellationToken>()
        );

        refreshTokenRepository.RevokeSessionAsync(
            Arg.Do<Guid>(sessionId =>
            {
                foreach (var token in tokens.Where(x => x.SessionId == sessionId && x.RevokedAt is null))
                {
                    token.RevokedAt = DateTime.UtcNow;
                }
            }),
            Arg.Any<CancellationToken>()
        );

        refreshTokenRepository.DeleteExpiredTokensForUserAsync(
            Arg.Do<string>(userId =>
            {
                DateTime now = DateTime.UtcNow;
                tokens.RemoveAll(x => x.UserId == userId && (x.ExpiresAt <= now || x.RevokedAt is not null));
            }),
            Arg.Any<CancellationToken>()
        );
    }
}
