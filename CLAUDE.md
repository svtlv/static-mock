# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

SMock is a .NET library for mocking static and instance methods and properties. It's built on top of the MonoMod library and provides two distinct API styles:

- **Hierarchical Setup**: Mock setup with validation actions - `Mock.Setup(expression, action).Returns(value)`
- **Sequential Setup**: Disposable mock setup - `using var _ = Mock.Setup(expression).Returns(value)`

The library targets multiple frameworks: .NET Standard 2.0, .NET 11 (`net11.0`) and .NET Framework (4.62, 4.7, 4.71, 4.72, 4.8, 4.81), published as the "SMock" NuGet package. The test projects target .NET Framework 4.6.2 to 4.8.1, `net10.0` and `net11.0`. .NET 8 and .NET 9 are out of support and are no longer test targets.

The current version is the prerelease `2.7.0-preview.1`. It depends on the MonoMod prerelease `MonoMod.Core 1.4.0-prerelease.3`, because MonoMod has no stable release with a `net11.0` target yet. A stable SMock `2.7.0` follows when MonoMod is stable.

## Prerequisites

The development environment requires:
- .NET 10.0 and 11.0 SDKs installed for cross-platform testing
- All tests run on Windows by default; Unix/macOS testing requires framework-specific commands (macOS and Linux cannot run the .NET Framework tests)

## Development Commands

### Build and Test
```bash
# Navigate to src directory first
cd src

# Restore dependencies
dotnet restore

# Build (Release configuration recommended)
dotnet build --configuration Release --no-restore

# Run all tests (Windows)
dotnet test --no-build --configuration Release --verbosity minimal

# Run tests for specific framework (Unix/macOS)
dotnet test --framework net10.0 --no-build --configuration Release --verbosity minimal
dotnet test --framework net11.0 --no-build --configuration Release --verbosity minimal

# Run a single test class
dotnet test --filter "ClassName=SetupMockReturnsTests"

# Run tests with specific category
dotnet test --filter "TestCategory=Hierarchical"
```

### Working with Individual Projects
```bash
# Build only the main library
dotnet build src/StaticMock/StaticMock.csproj

# Run only unit tests
dotnet test src/StaticMock.Tests/StaticMock.Tests.csproj

# Run benchmarks
dotnet run --project src/StaticMock.Tests.Benchmark/StaticMock.Tests.Benchmark.csproj
```

### Package Building and Publishing

Official releases are triggered by pushing a `vX.Y.Z` (or prerelease) tag.
The tag must match the project `<Version>` and a dated section in `CHANGELOG.md`.
Follow `docs/releasing.md`; `.github/workflows/release.yaml` tests, packs, publishes
to NuGet and GitHub Packages, and creates the GitHub release. Keep changes under
`Unreleased` until preparing a specific release.

Every pull request into `master` also runs two checks. The release metadata check in `release.yaml` runs
`scripts/prepare-release.ps1` for the project `<Version>`. It fails when `CHANGELOG.md` has no dated section for that version.
The docs workflow builds DocFX without a deploy. Neither check packs or publishes.

```bash
# Pack the library for NuGet publication
dotnet pack --configuration Release --output packages src/StaticMock/StaticMock.csproj

# Validate release metadata before tagging (PowerShell)
pwsh -File scripts/prepare-release.ps1 -Tag vX.Y.Z
```

## Architecture Overview

### Core Components

**Mock Entry Points** (`Mock.Hierarchical.cs`, `Mock.Sequential.cs`):
- Partial class split into two files for the two API styles
- All setup methods route through `SetupMockHelper` for consistency

**Hook Management System**:
- `HookBuilderFactory`: Determines whether to create static or instance hook builders
- `StaticHookBuilder`/`InstanceHookBuilder`: Create method hooks using MonoMod
- `MonoModHookManager`: Manages hook lifecycle and method interception
- `HookBuilderHelper` emits the hook of an instance method as a static method. Its first parameter is the declaring type (`ref T` for a value type). MonoMod.Core 1.4 maps the original `this` onto this parameter. `TestStructInstance` tests the value type path.

**Mock Implementations** (`Mocks/`):
- Hierarchical mocks: Support inline validation during setup
- Sequential mocks: Return disposable objects for automatic cleanup
- Type-specific mocks: `IFuncMock<T>`, `IAsyncFuncMock<T>`, `IActionMock`

**Context System** (`Entities/Context/`):
- `SetupContext`: Provides access to `It` parameter matching
- `It`: Argument matchers like `IsAny<T>()`, `Is<T>(predicate)`

### Key Design Patterns

**Factory Pattern**: `HookBuilderFactory` creates appropriate builders based on static vs instance methods

**Expression Tree Processing**: Converts Lambda expressions into `MethodInfo` for runtime hook installation

**Disposable Pattern**: Sequential mocks implement `IDisposable` for automatic cleanup

## Testing Structure

### Test Organization
- `Tests/Hierarchical/`: Tests for hierarchical API style
- `Tests/Sequential/`: Tests for sequential API style
- `Tests/*/ReturnsTests/`: Mock return value functionality
- `Tests/*/ThrowsTests/`: Exception throwing functionality
- `Tests/*/CallbackTests/`: Callback execution tests

### Test Entities (`StaticMock.Tests.Common/TestEntities/`):
- `TestStaticClass`: Static methods for testing
- `TestStaticAsyncClass`: Async static methods
- `TestInstance`: Instance methods
- `TestGenericInstance`: Generic type testing
- `TestRuntimeAsyncClass` / `TestRuntimeAsyncInstance`: runtime-async `Task`, `Task<T>`, `ValueTask` and `ValueTask<T>` methods (only under `#if NET11_0_OR_GREATER`)

### Running Specific Test Scenarios
```bash
# Test hierarchical returns functionality
dotnet test --filter "FullyQualifiedName~Hierarchical.ReturnsTests"

# Test sequential callback functionality
dotnet test --filter "FullyQualifiedName~Sequential.CallbackTests"

# Test async functionality across both styles
dotnet test --filter "FullyQualifiedName~Async"
```

`src/StaticMock.Tests/StaticMock.Tests.runsettings` (set by `RunSettingsFilePath` in the test project) sets `RunConfiguration.DisableAppDomain` to `true`. Keep it. Without it, the .NET Framework tests run in a separate AppDomain. A `DateTime.Now`/`UtcNow` throws mock patches shared `mscorlib` code. The adapter reads the time in the default AppDomain on the NUnit `EventPump` thread, and then builds the stack trace of the mocked exception. The CLR finds no hook assembly there and crashes (access violation in `Assembly::GetSecurityDescriptor`). `NumberOfTestWorkers=0` and `[NonParallelizable]` do not prevent this.

The NUnit VSTest adapter does not support `TestMethod` as a filter property. Use `FullyQualifiedName`, `Name`, `ClassName` or `TestCategory`.

## Multi-Framework Support

The project targets multiple .NET versions to ensure broad compatibility:
- **netstandard2.0**: Uses `MonoMod.Core` 1.4.0-prerelease.3 and `System.Reflection.Emit`
- **net11.0**: Uses `MonoMod.Core` 1.4.0-prerelease.3
- **.NET Framework 4.62-4.81**: Uses `MonoMod.Core` 1.4.0-prerelease.3 (the same `DetourFactory` hook code as the other targets; `MonoMod.RuntimeDetour` is no longer used)

When adding new features, ensure compatibility across all target frameworks by checking conditional compilation in the `.csproj` files.

### Runtime-async (.NET 11)

- The `net11.0` build of the test projects uses `<Features>$(Features);runtime-async=on</Features>`. The library has no `async` methods, so this flag changes no library code.
- MonoMod.Core 1.4 supports hooks on runtime-async methods (MonoMod PR 346). The tests in `Tests/*/RuntimeAsyncTests/` prove that SMock hooks `Task`, `Task<T>`, `ValueTask` and `ValueTask<T>` methods in both API styles.
- `async void` is not runtime-async. Keep the compatibility tests for it.

### Known limitation on .NET 11

On the .NET 11 runtime, SMock cannot mock generic method instantiations that return `Task`, `Task<T>`, `ValueTask` or `ValueTask<T>` (for example `Task.FromResult<int>`). MonoMod.Core 1.4.0-prerelease.3 does not support them yet. SMock throws a clear `NotSupportedException` and keeps the MonoMod exception as `InnerException`. Non-generic async methods and all other target frameworks work. When MonoMod supports this case, the limitation tests fail: then remove the skips of the three `AsyncExamples` and the limitation tests.

### Code Style

- Use modern C# features (primary constructors, collection expressions, `is null`, switch expressions, file-scoped namespaces) for all target frameworks. `LangVersion latest` compiles them for `netstandard2.0` and .NET Framework.
- Put polyfills for missing BCL types in `src/StaticMock/Polyfills/` and compile them out where the BCL has the type.
- Use .NET 11 and other new BCL APIs only under `#if NET11_0_OR_GREATER` (or the matching `NETx_0_OR_GREATER` symbol) and keep the old code path for `netstandard2.0` and .NET Framework. Do not add caches or locks only to use new types.
- Keep the hook code (`HookBuilderHelper`, `MonoModHookFactory`, the dynamic replacement methods, IL emit) logically identical. Change only syntax there. Add no inlining attributes.
- `src/.editorconfig` sets the modern-syntax rules as build warnings (`EnforceCodeStyleInBuild`). Keep the build at 0 warnings.
- Use `Guard.ThrowIfNull(value, nameof(value))` (`src/StaticMock/Helpers/Guard.cs`) for null checks. It calls `ArgumentNullException.ThrowIfNull` where the BCL has it.
- Make no public API change in a modernization. Build `master` and this branch in Release, dump the public types and members of `StaticMock.dll` for each target with reflection, and compare the two dumps.

## Documentation

API documentation is generated using DocFX:
- Configuration: `docfx_project/docfx.json`
- Published to: https://svtlv.github.io/static-mock/

To work with documentation locally, use the DocFX toolchain in the `docfx_project/` directory.

## Claude Code Plugin

A Claude Code plugin lives in `smock-plugin/` and is installable from any project via:

```
/plugins install github:svtlv/static-mock
```

Then invoke with `/smock-plugin:smock-tests [sequential|hierarchical]` to scaffold or update tests using SMock. The skill discovers the existing test structure, writes tests, updates the project's CLAUDE.md, and validates compilation.

## SMock Usage Policy

When writing any tests in this repository, **always use the SMock library** (`using StaticMock;`) for mocking. Never use manual workarounds, wrapper classes, or other mocking approaches for method interception.

### API Style Choice

- **Sequential API** (default for most tests): `using var mock = Mock.Setup(() => Target.Method()).Returns(value);`
  - Use `using var` — mandatory for cleanup
  - Best for straightforward return-value mocking
- **Hierarchical API** (when inline validation is needed): `Mock.Setup(() => Target.Method(), () => { /* assert inside */ }).Returns(value);`
  - No disposal needed — cleanup is automatic
  - Use when you need to assert on mock behavior inline

### Non-Negotiable Rules

1. Sequential mocks **must** use `using var` — omitting it leaves hooks active and corrupts subsequent tests
2. Always access parameter matchers through the context lambda: `context.It.IsAny<T>()`, never standalone `It`
3. Test projects **must** include `<Optimize>false</Optimize>` in `.csproj` to prevent compiler inlining from breaking hooks
4. Async methods: prefer `.ReturnsAsync(value)` (shorthand on `IAsyncFuncMock<T>`); alternatively `.Returns(Task.FromResult(value))` or `.Returns(async () => value)`
5. Void methods (`IActionMock`) use `.Callback<T>(action)` for side effects — `Callback` does **not** exist on `IFuncMock`
6. The hierarchical validation parameter is an `Action`. Never pass an `async` lambda to it: it becomes `async void`, and the hook can end before the assertion finishes. Keep the validation synchronous. If it needs an async call, prefer the sequential API and `await` the call inside the `using` scope. Block on the call with `.GetAwaiter().GetResult()` only when the call returns an already completed task (for example, the mock returns a completed task). Never block on a pending task: a `SynchronizationContext` or a custom `TaskScheduler` can stop its continuation and cause a deadlock.
7. A mock is process-wide, and NUnit and the test adapter read `DateTime.Now`/`UtcNow` on their own threads. Never use `Throws` on `DateTime.Now`, `DateTime.UtcNow` or other shared framework members. Mock an owned wrapper instead (`TestStaticClass.Now`, `TestStaticClass.UtcNow`). `Returns` on them is safe in a short scope.

### Test Naming Convention

- Test project: `OriginalProjectName.Tests`
- Test class: `OriginalClassNameTests`
- Test method: `Test` + MethodName [+ ScenarioDescription] (e.g., `TestGetUserReturnsActiveUser`)
