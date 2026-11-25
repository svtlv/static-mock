# Migration Guide

This guide helps you migrate between different versions of SMock and provides guidance for upgrading from other mocking frameworks.

## Table of Contents
- [Version Migration](#version-migration)
- [Upgrading to 2.7.0-preview.1](#upgrading-to-270-preview1)
- [Upgrading from Other Mocking Frameworks](#upgrading-from-other-mocking-frameworks)

## Version Migration

### Upgrading to Latest Version

When upgrading SMock, always check the [release notes](https://github.com/svtlv/static-mock/releases) for breaking changes.

#### Package Update Commands

```powershell
# Package Manager Console
Update-Package SMock

# .NET CLI
dotnet add package SMock --version [latest-version]

# Check current version
dotnet list package SMock
```

### Upgrading to 2.7.0-preview.1

SMock 2.7.0-preview.1 adds .NET 11 support. It is a **prerelease**, because it depends on the MonoMod prereleases (`MonoMod.Core 1.4.0-prerelease.3`). MonoMod has no stable release with a `net11.0` target yet. A stable SMock 2.7.0 follows when MonoMod is stable.

`Version="*"` and `dotnet add package SMock` select stable versions only. Install the prerelease with an explicit version or with `--prerelease`:

```bash
dotnet add package SMock --version 2.7.0-preview.1
# or install the latest prerelease
dotnet add package SMock --prerelease
```

```xml
<PackageReference Include="SMock" Version="2.7.0-preview.1" />
```

#### What changed

| Area | Change |
|------|--------|
| New target | The library targets `net11.0` (next to `netstandard2.0` and .NET Framework 4.6.2-4.8.1) |
| Dropped test targets | The tests no longer run on .NET 8 and .NET 9 (out of support). They run on .NET 10, .NET 11 and .NET Framework 4.6.2-4.8.1 |
| MonoMod | The library uses `MonoMod.Core 1.4.0-prerelease.3` for all targets. `MonoMod.RuntimeDetour` is no longer a dependency |
| Runtime-async | SMock hooks runtime-async methods on .NET 11 (`Task`, `Task<T>`, `ValueTask` and `ValueTask<T>`), in both API styles |
| Public API | No public API change. The code modernization (C# 14/15 and .NET 11 APIs) changes no behavior |

The library still has a `netstandard2.0` target, which .NET 8 and .NET 9 projects can use. SMock no longer tests on those runtimes, because they are out of support.

#### Known limitation on .NET 11

On the .NET 11 runtime, SMock cannot mock generic method instantiations that return `Task`, `Task<T>`, `ValueTask` or `ValueTask<T>` (for example `Task.FromResult<int>`), because MonoMod.Core 1.4.0-prerelease.3 does not support them yet. SMock throws a clear `NotSupportedException` and keeps the MonoMod exception as `InnerException`. Non-generic async methods and all other target frameworks work. If you mock such a method, mock a non-generic wrapper in your own code instead.

#### Keep hierarchical validation synchronous

The validation parameter of the hierarchical `Mock.Setup` is an `Action`. Do not pass an `async` lambda to it: it becomes `async void`, and the hook can end before the assertion finishes. Block on the async call inside the validation (for example with `.GetAwaiter().GetResult()`).

#### If you use NUnit

If you move your own tests to NUnit 5, `Assert.ThrowsAsync` and the other async asserts return a `Task`. Await them. In a sequential test, await them before the `using var` mock is disposed.

## Upgrading from Other Mocking Frameworks

### From Moq

SMock can complement Moq for static method scenarios. Here's how to migrate common patterns:

#### Basic Mocking
```csharp
// Moq (interface/virtual methods only)
var mock = new Mock<IFileService>();
mock.Setup(x => x.ReadFile("test.txt")).Returns("content");

// SMock (static methods)
using var mock = Mock.Setup(() => File.ReadAllText("test.txt"))
    .Returns("content");
```

#### Parameter Matching
```csharp
// Moq
mock.Setup(x => x.Process(It.IsAny<string>())).Returns("result");

// SMock
using var mock = Mock.Setup((context) => MyClass.Process(context.It.IsAny<string>()))
    .Returns("result");
```

#### Callback Verification
```csharp
// Moq
var callCount = 0;
mock.Setup(x => x.Log(It.IsAny<string>()))
    .Callback<string>(msg => callCount++);

// SMock
var callCount = 0;
using var mock = Mock.Setup((context) => Logger.Log(context.It.IsAny<string>()))
    .Callback<string>(msg => callCount++);
```

#### Exception Throwing
```csharp
// Moq
mock.Setup(x => x.Connect()).Throws<ConnectionException>();

// SMock
using var mock = Mock.Setup(() => DatabaseHelper.Connect())
    .Throws<ConnectionException>();
```

### From NSubstitute

```csharp
// NSubstitute (interfaces only)
var service = Substitute.For<IDataService>();
service.GetData("key").Returns("value");

// SMock (static methods)
using var mock = Mock.Setup(() => StaticDataService.GetData("key"))
    .Returns("value");

// NSubstitute - Parameter matching
service.GetData(Arg.Any<string>()).Returns("value");

// SMock - Parameter matching
using var mock = Mock.Setup((context) => StaticDataService.GetData(context.It.IsAny<string>()))
    .Returns("value");
```

### From Microsoft Fakes (Shims)

Microsoft Fakes Shims are similar to SMock but with different syntax:

```csharp
// Microsoft Fakes Shims
[TestMethod]
public void TestWithShims()
{
    using (ShimsContext.Create())
    {
        System.IO.Fakes.ShimFile.ReadAllTextString = (path) => "mocked content";

        // Test code here
    }
}

// SMock equivalent
[Test]
public void TestWithSMock()
{
    using var mock = Mock.Setup((context) => File.ReadAllText(context.It.IsAny<string>()))
        .Returns("mocked content");

    // Test code here
}
```

#### Key Differences:
- **SMock** uses familiar lambda syntax like other modern mocking frameworks
- **SMock** supports both sequential and hierarchical APIs
- **SMock** has built-in parameter matching with `It` class
- **SMock** works with any test framework, not just MSTest

## Getting Help with Migration

If you encounter issues during migration:

1. **Check Release Notes**: Review the specific version's release notes for known issues
2. **Search Issues**: Check [GitHub Issues](https://github.com/svtlv/static-mock/issues) for similar problems
3. **Community Support**: Ask in [GitHub Discussions](https://github.com/svtlv/static-mock/discussions)
4. **Create Issue**: If you find a bug, create a detailed issue with:
   - Source and target versions
   - Minimal reproduction code
   - Error messages and stack traces
   - Environment details (.NET version, OS, etc.)

## Post-Migration Best Practices

After successful migration:

- **Run Full Test Suite**: Ensure all tests pass with the new version
- **Performance Testing**: Compare test execution times before and after
- **Code Review**: Review mock setups for optimization opportunities
- **Documentation Update**: Update team documentation with new patterns
- **Training**: Share new features and patterns with your team

This migration guide should help you smoothly transition between SMock versions and from other mocking frameworks. For additional support, consult the [troubleshooting guide](troubleshooting.md).

## Working Migration Examples

The migration examples shown in this guide are based on actual working test cases. You can find complete, debugged migration examples in the SMock test suite:

- **[Migration Examples](https://github.com/svtlv/static-mock/blob/master/src/StaticMock.Tests/Tests/Examples/MigrationGuide/MigrationExamples.cs)** - `src/StaticMock.Tests/Tests/Examples/MigrationGuide/MigrationExamples.cs`

These examples demonstrate:
- **Current working syntax** - All examples compile and pass tests with the latest SMock version
- **Best practices** - Proper usage patterns for both Sequential and Hierarchical APIs
- **Real-world scenarios** - Practical migration patterns you can copy and adapt
- **Parameter matching** - Up-to-date syntax for `It.IsAny<T>()` and other matchers

### Running Migration Examples

```bash
# Navigate to the src directory
cd src

# Run the migration examples specifically
dotnet test --filter "ClassName=MigrationExamples"

# Or run all example tests
dotnet test --filter "FullyQualifiedName~Examples"
```