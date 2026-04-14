# ATT - API Test Template Guide

## What is ATT?

ATT is a dotnet template that scaffolds an API test project using XUnit and Playwright. You install the template once, then use it to generate new test projects targeting any API.

## Installation

```bash
# Clone the repo and install the template
dotnet new install . --force
```

## Creating a new project

```bash
mkdir MyApiTests
cd MyApiTests
dotnet new att --apiUrl https://your-api.com
```

The `--apiUrl` parameter is required. It sets the base URL your tests will run against. The project namespace will match your folder name.

## How BaseURL works

The base URL flows through the project like this:

1. `appsettings.json` stores it under `TestSettings:BaseURL`
2. `AppConfig.cs` reads it from the config file and exposes it as `AppConfig.BaseURL`
3. `PlaywrightDriver.cs` passes it to Playwright's `APIRequestNewContextOptions.BaseURL` — this is a built-in Playwright property, not something custom to this template
4. Playwright resolves all relative paths against it, so endpoint wrappers only need paths like `/users/1` instead of full URLs

## Running tests

```bash
# Install Playwright browsers (first time only)
pwsh bin/Debug/net10.0/playwright.ps1 install

# Run all tests
dotnet test

# Run a single test
dotnet test --filter "FullyQualifiedName~UsersTests.GetUser_AlwaysRuns"

# Run all tests in a class
dotnet test --filter "FullyQualifiedName~UsersTests"
```

## Test attributes

ATT provides three ways to define tests:

### `[Fact]` — standard XUnit, always runs

```csharp
[Fact]
public async Task GetUser_AlwaysRuns()
{
    var (response, data) = await _ePUsers.GetUser(1);
    Assert.Equal(200, response.Status);
}
```

Nothing special here. The test runs every time.

### `[JsonFact]` — runs only if registered in `testrunner_facts.json`

```csharp
[JsonFact("UsersTests")]
public async Task GetUser_ResponseStatusIsOK()
{
    var (response, data) = await _ePUsers.GetUser(1);
    Assert.Equal(200, response.Status);
}
```

The string parameter is a category key, not a class name. It can be anything — it just needs to match a key in `testrunner_facts.json`:

```json
{
  "UsersTests": [
    "GetUser_ResponseStatusIsOK"
  ],
  "SmokeTests": [
    "GetUser_SmokeTest"
  ]
}
```

Tests not listed here are skipped. This lets you control which tests run without changing code.

### `[Theory]` + `[JsonInline]` — parameterized tests with data from JSON

```csharp
[Theory]
[JsonInline("UsersTests")]
public async Task GetUserById_ResponseStatusIsOK(int userId)
{
    var (response, data) = await _ePUsers.GetUser(userId);
    Assert.Equal(200, response.Status);
}
```

Test data comes from `testrunner_theories.json`:

```json
{
  "UsersTests": {
    "GetUserById_ResponseStatusIsOK": [
      [1],
      [2],
      [3]
    ]
  }
}
```

The method **must** be registered in the JSON file. Unlike `[JsonFact]` which skips unregistered tests, a `[JsonInline]` test missing from the JSON will crash.

Supported parameter types: `int`, `long`, `string`, `bool`, `double`, `decimal`, and any type that can be deserialized from JSON.

## Writing endpoint wrappers

Endpoint wrappers live in `TestFiles/Endpoints/`. They take a `PlaywrightDriver` and expose typed methods for each API call:

```csharp
public class EPUsers
{
    private PlaywrightDriver _playwrightDriver;

    public record UserRecord(int id, string name, string username, string email);

    public EPUsers(PlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver;
    }

    public async Task<(IAPIResponse, UserRecord)> GetUser(int userId)
    {
        var response = await _playwrightDriver.ApiRequestContext.GetAsync($"/users/{userId}");
        var data = await response.JsonAsync<UserRecord>()
            ?? throw new InvalidOperationException("Data was null here!");
        return (response, data);
    }
}
```

The pattern is: make the request via `ApiRequestContext`, deserialize with `JsonAsync<T>()`, return both the response and the typed data. Tests then assert on status codes and data separately.

## Writing test classes

Test classes use `IClassFixture<PlaywrightDriver>` to get a shared Playwright instance. Create endpoint wrappers in the constructor:

```csharp
public class UsersTests : IClassFixture<PlaywrightDriver>
{
    private PlaywrightDriver _playwrightDriver;
    private EPUsers _ePUsers;

    public UsersTests(PlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver;
        _ePUsers = new(_playwrightDriver);
    }
}
```

## Formatting

This template recommends [CSharpier](https://csharpier.com/) for code formatting. Install it as a dotnet tool or VS Code extension. It's an opinionated formatter — no configuration needed.
