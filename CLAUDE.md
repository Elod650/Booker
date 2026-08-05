# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

### Build
```powershell
dotnet build Booker.slnx
```

### Run the backend API
```powershell
dotnet run --project src/Booker.Backend
```

### Run the Blazor frontend
```powershell
dotnet run --project src/Booker.Clients.Blazor.Server
```

### Run all tests
```powershell
dotnet run --project tests/Services.UnitTests
```

The test projects are TUnit/Microsoft.Testing.Platform executables. `dotnet test` currently fails on
the .NET 10 SDK ("Testing with VSTest target is no longer supported") because the solution has not
opted into the new `dotnet test` experience — run the test project directly instead.

### Run a single test class
```powershell
dotnet run --project tests/Services.UnitTests --treenode-filter "/*/*/CalendarServiceTests/*"
```

### Format code (CSharpier)
```powershell
dotnet csharpier format .
```

## Architecture

Booker is a professional scheduling/booking platform. The solution is split into these projects:

### `src/Booker.Backend`
ASP.NET Core Web API. Entry point (`Program.cs`) wires up three extension methods: `ConfigureDatabase()`, `ConfigureServices()`, `ConfigureAuthentication()`. In Development mode it seeds an **in-memory EF Core database** with roles, calendars, and customer relationships. OpenAPI docs are served via Scalar at `/scalar`.

Controllers: `AppointmentController`, `AuthController`, `CalendarController`, `InfoController`, `ServiceController`. All are `[Authorize]` by default; some actions further restrict to `"Admin"` or `"Admin, Provider"` roles. The user identity sub-claim (`JwtRegisteredClaimNames.Sub`) is used throughout to scope actions to the current user.

`ValidatorService` is used by controllers to enforce access to a calendar. Two levels exist:

- **Ownership** (`ValidateCalendarOwnership` / `ValidateServiceOwnership` / `ValidateAppointmentOwnership`) — the requesting user must be the calendar's owner. Used for anything that mutates the calendar itself, its services, or its customer list.
- **Access** (`ValidateCalendarAccess`) — the calendar's owner *or* a customer invited to it via `CalendarsXCustomers`. Booking is invite-only, so `AppointmentController.AddAppointment` gates on this rather than ownership. Backed by `ICalendarRepository.IsCustomerOnCalendarAsync`, an `AnyAsync` membership probe.

AutoMapper maps between entities and DTOs/requests. All mappings are declared in `AutoMapperConfig.cs`.

### `src/Booker.Services`
Business logic layer. Each domain area has an interface (`Interfaces/`) and implementation (`Services/`):
- `AppointmentService`, `CalendarService`, `ServiceService`, `InfoService`, `AuthService`, `ValidatorService`

`AuthService` handles JWT + refresh token generation. JWT config is bound from `appsettings.json` into `JwtOptions`.

**Appointment times are local wall-clock.** `Appointment.StartTime`/`EndTime` carry no timezone and are interpreted in the deployment's local zone (single-region: Hungary). This is deliberate — `Calendar.StartTime`/`EndTime` work hours are zone-less strings (`"08:00"`), and `ValidateBookingRules` compares the two directly, so converting appointments to UTC would offset the work-hours check. Anything comparing against `Appointment.StartTime` must use `DateTime.Now`, not `DateTime.UtcNow`. Unrelated: token expiry (`AuthService`, `RefreshTokenRepository`) and audit columns (`AppDbContext.ApplyTimestamps`) are absolute instants and correctly stay on `DateTime.UtcNow`. Going multi-region means adding a `TimeZoneId` to `Calendar` and converting at the boundary — not flipping these comparisons to UTC.

**Refresh tokens** are stored one row per session in the `RefreshTokens` table, never in plaintext — only a SHA-256 hash (`TokenHasher.ComputeHash`) is persisted. A `SessionId` is stamped at login and carried through every rotation, so one user can hold several concurrent sessions (web + mobile) and a single session can be revoked on its own. Every refresh rotates the token and marks the old row `RevokedAt`; presenting an already-revoked token is treated as a replay and revokes that whole session. Expired/revoked rows are cleaned up opportunistically on login.

### `src/Booker.Repository`
EF Core data layer. `AppDbContext` extends `IdentityDbContext<ApplicationUser>` and exposes `DbSet`s for `Appointments`, `Calendars`, `Services`, `Infos`, `CalendarsXCustomers`, and `RefreshTokens`.

Entities inherit from `EntityBase` (auto-increment `int Id`). Repositories follow an interface-per-aggregate pattern (`IAppointmentRepository`, `ICalendarRepository`, etc.).

**Tracking**: every entity-returning `Get…Async` takes an optional `bool asNoTracking = false`. The default is **tracked**, so entities fetched for an update or delete work without extra ceremony; read-only paths (anything that just maps to a DTO) pass `asNoTracking: true`. `GetCalendarIdsAsync` has no such parameter because it projects to a scalar.

**Cascade deletes** are declared once, in the EF model (`Configurations/`): `Appointment → Calendar`, `Appointment → Service`, `Service → Calendar`, `CalendarsXCustomers → Calendar`/`Customer` and `RefreshToken → User` are all `DeleteBehavior.Cascade`. Repositories never delete dependents by hand — `DeleteCalendarAsync`/`DeleteServiceAsync` just `Load()` the dependent navigations so the change tracker can apply that cascade, because the in-memory provider only cascades to dependents it already tracks. The one deliberate exception is `RemoveCustomerFromCalendarAsync`, which drops the customer's *upcoming* appointments — that's a business rule, not a cascade.

### `src/Booker.Models`
Shared model library (no ASP.NET dependency). Contains:
- **DTOs** (`AppointmentDto`, `CalendarDto`, `ServiceDto`, `UserDto`, `AuthResponse`) — outgoing responses
- **Requests** (`EditCalendarRequest`, `EditAppointmentRequest`, etc.) — incoming payloads with data-annotation validation
- **Enums** — `RolesEnum` with `Admin`, `Provider`, `Customer`

### `src/Booker.ApiCaller`
Framework-agnostic HTTP client library for calling the backend. `ApiCallerBase` handles:
- Bearer token injection
- Automatic token refresh on 401 (calls the `/api/auth/refresh` endpoint, updates stored tokens)
- Forced logout when refresh also fails

Callers for each controller live in `CallsForControllers/` (`AppointmentApiCaller`, `AuthApiCaller`, etc.).

Consumers must call `SetBasicData()` once, supplying delegates for getting/setting tokens and triggering logout. In the Blazor app this is wired up in `ApiCallerMediator`.

### `src/Booker.Clients.Blazor.Server`
Blazor Server frontend (interactive server render mode). Pages are under `Components/Pages/` organized by domain: `Auth/`, `Booking/`, `Calendars/`, `Services/`.

**Authentication** is custom: `CustomAuthStateProvider` parses claims directly from the stored JWT (no cookie). Tokens are stored via `SessionStorageManager` (ASP.NET `ProtectedSessionStorage`).

**ApiCallerMediator** (`Helpers/ApiCallerMediator.cs`) acts as the bootstrap glue — it reads the refresh URL from config and wires the `ApiCallerBase` delegates to `CustomAuthStateProvider` methods. It must be instantiated early (registered as a singleton) to configure the HTTP layer before any API calls.

ViewModels (`EditCalendarViewModel`, `EditServiceViewModels`, `SchedulerAppointmentViewModel`) adapt DTOs/requests for Blazor component binding and the Syncfusion scheduler.

The Syncfusion `SfSchedule` component is used for the calendar/booking view. It requires a registered license key (`LicenseKeys:Syncfusion` in config).

### `tests/Services.UnitTests`
Unit tests for service-layer classes using **TUnit** (assertion framework) and **NSubstitute** (mocking). Tests instantiate services directly with substituted repository/UserManager dependencies. Test data helpers live in `TestData/` and `Helpers/`.

## Key conventions

- **Role strings** in `[Authorize(Roles = "...")]` must match the string values of `RolesEnum` exactly (`Admin`, `Provider`, `Customer`).
- **Error returns**: services return `null` on success and an error-message `string` on failure. Controllers pattern-match on `null` to choose between `Ok()` and `BadRequest(errorMessage)`.
- **User ID** is always the JWT `sub` claim, extracted with `User.FindFirstValue(JwtRegisteredClaimNames.Sub)`.
- **Database**: currently in-memory only. EF migrations are not used; `EnsureCreated()` seeds on startup in Development.
- **Central package management**: all NuGet versions are pinned in `Directory.Packages.props`; individual `.csproj` files omit `Version` attributes.
- **Target framework**: .NET 10.
