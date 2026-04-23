# Booker

## Solution Overview

Booker is a multi-project .NET 10 solution for book appointments for individual providers (e.g. hair dressers, private coaches). 

### Key Projects

- **Models:** Entities, enums, and other models.
- **Services:** The business logic.
- **Repository:** Entity Framework Core DbContext, repositories, and migrations.
- **Backend:** ASP.NET Core Web API project exposing REST endpoints.
- **Clients:** Frontend clients (e.g., Blazor WebAssembly, WPF, MAUI) consuming the backend API.
- **ApiCaller:** A shared library for making HTTP calls to the backend API, used by .NET based frontend clients.

## Development Guidelines

### .NET Development (Backend and .NET Frontend)

**MANDATORY: Follow these instructions for all backend development tasks.**

- **C# Standards:** See [.github/instructions/csharp.instructions.md](.github/instructions/csharp.instructions.md)

### Backend Commands

Always run the build, tests, and formatter after making changes to the backend. Fix any error or warning before completing the task. Not running these checks locally will lead to incomplete or broken implementation.

```bash
# From the repository root
dotnet build Byx.sln
dotnet test Byx.sln
dotnet csharpier format .
```

## Testing Conventions

- **Assertions:** TUnit async/fluent assertion model (e.g., `await Assert.That(result).IsNotNull();`, `await Assert.That(value).IsEqualTo(expected);`). Do not use NUnit-style `Assert.That(x, Is.EqualTo(y))`.
- **Test project naming:** `<ProjectName>.UnitTests` in `tests/`.
- **Test class naming:** `<ClassUnderTest>Tests`.
- **Test method naming:** `MethodName_ShouldExpectedBehavior_WhenCondition`.
- **Helper methods:** Use private helper methods within the test class for setup and common assertions. Do not create separate utility classes for test helpers. Always place helper methods at the bottom of the test class, after all test methods.
