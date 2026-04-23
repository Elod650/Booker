# Authentication & Authorization Implementation Plan

## Problem Statement

The Booker application currently has **zero authentication or authorization**. All API endpoints and Blazor pages are publicly accessible. We need to implement a complete auth system using **ASP.NET Identity** for user management, **JWT tokens** (access + refresh) for API authentication, and a **Blazor Server login/register UI** with `sessionStorage` for token persistence.

## Chosen Approach

- **ASP.NET Identity** integrated into the existing `AppDbContext` (InMemory database for now)
- **JWT Bearer authentication** on the Backend API
- **Refresh token** support (stored in Identity database)
- **Three roles**: Admin, Provider, Customer
- **All endpoints** require authentication (except login/register)
- **Blazor Server** client: Login + Registration pages, `AuthenticationStateProvider` backed by `sessionStorage`
- **Unit tests** for the auth service

## Architecture Overview

```
Blazor Server Client                   Backend API
┌─────────────────────┐               ┌──────────────────────┐
│ Login/Register Pages│──HTTP POST───▶│ AuthController       │
│ AuthStateProvider   │               │  POST /api/auth/login│
│ sessionStorage      │◀──JWT────────│  POST /api/auth/register│
│ (tokens)            │               │  POST /api/auth/refresh│
│                     │               │                      │
│ ApiCaller (+Bearer) │──HTTP+JWT───▶│ [Authorize] endpoints│
│                     │               │ Identity + EF Core   │
└─────────────────────┘               └──────────────────────┘
```

## Detailed Implementation Todos

### Phase 1: Models & DTOs

**1. Add Auth DTOs to Booker.Models**

- `LoginRequest` — `Email`, `Password` (in `Requests/`)
- `RegisterRequest` — `Email`, `Password`, `ConfirmPassword`, `FirstName`, `LastName` (in `Requests/`)
- `AuthResponse` — `AccessToken`, `RefreshToken`, `Expiration` (in `DTOs/`)
- `RefreshTokenRequest` — `RefreshToken` (in `Requests/`)
- Update `GlobalUsings.cs` if needed

### Phase 2: Repository — Identity Integration

**2. Add ASP.NET Identity to Booker.Repository**

- Add NuGet packages to `Directory.Packages.props`:
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
  - `Microsoft.Extensions.Identity.Core`
  - `Microsoft.Extensions.Identity.Stores`
- Add NuGet references to `Booker.Repository.csproj`:
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- Create `Entities/ApplicationUser.cs` extending `IdentityUser` with `FirstName`, `LastName`, `RefreshToken`, `RefreshTokenExpiryTime`
- Change `AppDbContext` to inherit from `IdentityDbContext<ApplicationUser>` instead of `DbContext`
- Update `GlobalUsings.cs` with Identity namespaces

**3. Seed Identity Roles and Admin User**

- Add role seeding (Admin, Provider, Customer) to `SeedData.cs`
- Add a default admin user seed (admin@booker.com / Admin123!)
- Update `DatabaseExtensions.cs` to seed roles and admin user after db creation

### Phase 3: Services — Auth Business Logic

**4. Create Auth Service in Booker.Services**

- Add NuGet packages to `Directory.Packages.props`:
  - `Microsoft.AspNetCore.Authentication.JwtBearer`
  - `System.IdentityModel.Tokens.Jwt` (if not pulled transitively)
- Add NuGet references to `Booker.Services.csproj`:
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
  - `Microsoft.Extensions.Options`
  - `Microsoft.AspNetCore.Authentication.JwtBearer`
- Create `JwtOptions.cs` — Options class for JWT settings (`SecretKey`, `Issuer`, `Audience`, `AccessTokenExpirationMinutes`, `RefreshTokenExpirationDays`)
- Create `Interfaces/IAuthService.cs`:
  - `Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)`
  - `Task<string?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)` — returns null on success, error message on failure
  - `Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken)`
- Create `Services/AuthService.cs` implementing `IAuthService`:
  - Inject `UserManager<ApplicationUser>`, `IOptions<JwtOptions>`
  - Login: validate credentials → generate JWT access token + refresh token → store refresh token on user → return `AuthResponse`
  - Register: create user via `UserManager` → assign default role (Customer)
  - RefreshToken: validate refresh token → generate new access + refresh tokens
  - Private helper: `GenerateAccessToken(ApplicationUser user, IList<string> roles)` — creates JWT with claims (sub, email, roles, jti)
  - Private helper: `GenerateRefreshToken()` — cryptographically random string
- Update `GlobalUsings.cs`

### Phase 4: Backend — JWT Configuration & Auth Controller

**5. Configure JWT Authentication in Backend**

- Add NuGet references to `Booker.Backend.csproj`:
  - `Microsoft.AspNetCore.Authentication.JwtBearer`
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- Update `Program.cs`:
  - Add `app.UseAuthentication()` before `app.UseAuthorization()`
  - Add Identity user/role seeding in the startup scope
- Create/update `Extensions/AuthExtensions.cs`:
  - Register Identity services: `AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>()`
  - Configure JWT Bearer: `AddAuthentication(JwtBearerDefaults)` → `AddJwtBearer(options)` with token validation parameters
  - Register `IAuthService` + `AuthService`
  - Bind `JwtOptions` from configuration
- Update `ServiceExtensions.cs` to call `ConfigureAuthentication()`
- Add JWT settings to `appsettings.json` (placeholder values) and `appsettings.Development.json` (real dev values):
  ```json
  "JwtOptions": {
    "SecretKey": "",
    "Issuer": "",
    "Audience": "",
    "AccessTokenExpirationMinutes": 0,
    "RefreshTokenExpirationDays": 0
  }
  ```

**6. Create AuthController**

- `POST api/auth/login` — `[AllowAnonymous]` — calls `IAuthService.LoginAsync`
- `POST api/auth/register` — `[AllowAnonymous]` — calls `IAuthService.RegisterAsync`
- `POST api/auth/refresh` — `[AllowAnonymous]` — calls `IAuthService.RefreshTokenAsync`
- Returns appropriate HTTP status codes (200, 400, 401)

**7. Protect Existing Controllers**

- Add `[Authorize]` attribute to all existing controllers (`AppointmentController`, `CalendarController`, `ServiceController`, `InfoController`)
- Optionally add role-based `[Authorize(Roles = "Admin")]` for specific destructive endpoints later (out of scope for now — all authenticated users get access)

### Phase 5: ApiCaller — JWT Token Support

**8. Update ApiCaller to Support Bearer Tokens**

- Add `AuthorizationToken` property to `ApiRequest` (optional)
- Update `ApiCallerBase.SendMessageAsync()` to add `Authorization: Bearer {token}` header when token is present
- Create `AuthApiCaller` / `IAuthApiCaller` in `CallsForControllers/`:
  - `LoginAsync(LoginRequest)` → `AuthResponse`
  - `RegisterAsync(RegisterRequest)` → `string?`
  - `RefreshTokenAsync(RefreshTokenRequest)` → `AuthResponse`
- Add `AuthApiUrl` to `ApiCallerOptions`
- Update existing ApiCallers to accept and pass tokens

### Phase 6: Blazor Server Client — Auth UI

**9. Add Auth Infrastructure to Blazor Client**

- Add NuGet reference: `Microsoft.AspNetCore.Components.Authorization`
- Create `Services/SessionStorageService.cs` — JS interop wrapper for `sessionStorage.getItem`, `setItem`, `removeItem`
- Create `Services/TokenAuthStateProvider.cs` — Custom `AuthenticationStateProvider`:
  - Reads JWT from sessionStorage
  - Parses claims from JWT to build `ClaimsPrincipal`
  - Exposes `NotifyUserAuthentication()` / `NotifyUserLogout()`
  - Handles token expiry detection
- Register in DI: `AuthenticationStateProvider` → `TokenAuthStateProvider`
- Register `IAuthApiCaller` → `AuthApiCaller`
- Update `App.razor` to wrap with `<CascadingAuthenticationState>`
- Update `Routes.razor` to use `<AuthorizeRouteView>` instead of `<RouteView>`

**10. Create Login Page**

- `Components/Pages/Auth/Login.razor` + `Login.razor.cs`
- Route: `/login`
- Form fields: Email, Password
- On submit: call `IAuthApiCaller.LoginAsync` → store tokens in sessionStorage → notify `AuthenticationStateProvider` → redirect to home
- Error display for invalid credentials
- Link to registration page

**11. Create Registration Page**

- `Components/Pages/Auth/Register.razor` + `Register.razor.cs`
- Route: `/register`
- Form fields: Email, Password, ConfirmPassword, FirstName, LastName
- On submit: call `IAuthApiCaller.RegisterAsync` → redirect to login on success
- Validation and error display
- Link to login page

**12. Update Layout & Navigation**

- Update `NavMenu.razor`: show Login/Register links when not authenticated, show Logout when authenticated
- Update `MainLayout.razor`: add auth state display (username) in top bar, add Logout button
- Add `[Authorize]` attribute to protected pages (Calendars, Services) — via `@attribute [Authorize]`
- Redirect unauthenticated users to `/login`

### Phase 7: Unit Tests

**13. Create Auth Service Unit Tests**

- Create `tests/Services.UnitTests/AuthServiceTests.cs`
- Test cases:
  - `LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid`
  - `LoginAsync_ShouldReturnNull_WhenUserNotFound`
  - `LoginAsync_ShouldReturnNull_WhenPasswordIsInvalid`
  - `RegisterAsync_ShouldReturnNull_WhenRegistrationSucceeds`
  - `RegisterAsync_ShouldReturnError_WhenEmailAlreadyExists`
  - `RefreshTokenAsync_ShouldReturnNewTokens_WhenRefreshTokenIsValid`
  - `RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenIsExpired`
- Mock `UserManager<ApplicationUser>` with NSubstitute
- Follow existing test patterns (TUnit assertions, `[Before(Test)]` setup)

### Phase 8: Configuration & Cleanup

**14. Create appsettings.Development.json Files**

- Backend: `appsettings.Development.json` with JWT dev secrets, connection strings
- Blazor: `appsettings.Development.json` with auth API URL
- Both added to `.gitignore` if not already

**15. Update Solution File**

- Ensure new test projects (if any) are included in `Booker.slnx`
- Verify `dotnet build Byx.sln` compiles
- Verify `dotnet test Byx.sln` passes
- Run `dotnet csharpier format .`

## Key Design Decisions

1. **InMemory database retained** — Identity tables exist in-memory; data is lost on restart. Sufficient for development. Migration to SQL Server/SQLite is a separate task.
2. **Refresh tokens stored on ApplicationUser** — Simple approach for MVP. Could be moved to a separate table for multi-device support later.
3. **sessionStorage** — Tokens cleared when browser tab closes. More secure than localStorage for this use case.
4. **All authenticated users have equal access** — Role-based endpoint restrictions (e.g., only Admin can delete) deferred to a future task. Roles exist in the system for future use.
5. **ApiCallerBase modified to support auth headers** — Token is passed per-request via `ApiRequest`. The Blazor client reads it from sessionStorage before each API call.

## Notes & Considerations

- The `ApiCallerBase` currently uses a static `HttpClient`. We need to carefully add the auth header per-request (via `HttpRequestMessage.Headers`) rather than on the shared client.
- Since the database is InMemory, the seeded admin user and roles are recreated on every app restart.
- JWT secret key must be at least 256 bits (32+ characters) for HMAC-SHA256.
- CORS may need configuration if Backend and Blazor run on different ports.
