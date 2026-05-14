# Booker

## Solution Overview

Booker is a multi-project .NET 10 solution for booking appointments for individual providers (e.g., hair dressers, private coaches).

### Key Projects

- **Models:** Entities, enums, and other models.
- **Services:** The business logic.
- **Repository:** Entity Framework Core DbContext, repositories, and migrations.
- **Backend:** ASP.NET Core Web API project exposing REST endpoints.
- **Clients:** Frontend clients (e.g., Blazor Server) consuming the backend API.
- **ApiCaller:** A shared library for making HTTP calls to the backend API, used by .NET-based frontend clients.

## Development Guidelines

### General Standards

- **Backend Commands:** Always run the build, tests, and formatter after making changes to the backend. Fix any error or warning before completing the task.
  ```powershell
  # From the repository root
  dotnet build Booker.slnx
  dotnet test Booker.slnx
  dotnet csharpier format .
  ```

## Testing Conventions

- **Framework:** TUnit.
- **Assertions:** Use async/fluent model (e.g., `await Assert.That(result).IsNotNull();`).
- **Naming:**
  - Projects: `<ProjectName>.UnitTests` in `tests/`.
  - Classes: `<ClassUnderTest>Tests`.
  - Methods: `MethodName_ShouldExpectedBehavior_WhenCondition`.
- **Helpers:** Private methods at the bottom of the test class.
- **Test Data:** Use `<ClassName>TestData` classes in `tests/Services.UnitTests/TestData/`.
