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
dotnet test Booker.slnx
```

### Run a single test file
```powershell
dotnet test tests/Services.UnitTests --filter "FullyQualifiedName~CalendarServiceTests"
```

### Format code (CSharpier)
```powershell
dotnet csharpier .
```

## Architecture

Booker is a professional scheduling/booking platform. The solution is split into these projects:

### `src/Booker.Backend`
ASP.NET Core Web API. Entry point (`Program.cs`) wires up three extension methods: `ConfigureDatabase()`, `ConfigureServices()`, `ConfigureAuthentication()`. In Development mode it seeds an **in-memory EF Core database** with roles, calendars, and customer relationships. OpenAPI docs are served via Scalar at `/scalar`.

Controllers: `AppointmentController`, `AuthController`, `CalendarController`, `InfoController`, `ServiceController`. All are `[Authorize]` by default; some actions further restrict to `"Admin"` or `"Admin, Provider"` roles. The user identity sub-claim (`JwtRegisteredClaimNames.Sub`) is used throughout to scope actions to the current user.

`ValidatorService` is used by controllers to enforce calendar ownership — any mutation on a calendar first checks that the requesting user is the calendar's owner.

AutoMapper maps between entities and DTOs/requests. All mappings are declared in `AutoMapperConfig.cs`.

### `src/Booker.Services`
Business logic layer. Each domain area has an interface (`Interfaces/`) and implementation (`Services/`):
- `AppointmentService`, `CalendarService`, `ServiceService`, `InfoService`, `AuthService`, `ValidatorService`

`AuthService` handles JWT + refresh token generation. JWT config is bound from `appsettings.json` into `JwtOptions`.

### `src/Booker.Repository`
EF Core data layer. `AppDbContext` extends `IdentityDbContext<ApplicationUser>` and exposes `DbSet`s for `Appointments`, `Calendars`, `Services`, `Infos`, and `CalendarsXCustomers`.

Entities inherit from `EntityBase` (auto-increment `int Id`). Repositories follow an interface-per-aggregate pattern (`IAppointmentRepository`, `ICalendarRepository`, etc.).

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
