# Changelog

Release history reconstructed from all repository version tags. Dates are the
tagged commits' dates; entries summarize the changes since the preceding tag.
Version-only commits and intermediate CI corrections are grouped with their
related changes. The Claude Code plugin has its own version; its changes after
the latest SMock tag are listed under `Unreleased`.

## Unreleased

### Improved

- SMock Tests plugin 0.4.0: tell the skill that a mock is process-wide. It must not use `Throws` on shared framework members such as `DateTime.Now` (mock an owned wrapper instead), and it adds a `.runsettings` file with `RunConfiguration.DisableAppDomain=true` to .NET Framework test projects. Both prevent test host crashes.

## v2.7.0-preview.1 - 2026-11-25

SMock 2.7.0-preview.1 is a prerelease. It depends on the MonoMod prereleases (`MonoMod.Core 1.4.0-prerelease.3`) for .NET 11 support, because MonoMod has no stable release with a `net11.0` target yet. A stable SMock 2.7.0 follows when MonoMod ships stable versions.

### New Features

- Add the `net11.0` target to the SMock package.
- Hook runtime-async methods that return `Task`, `Task<T>`, `ValueTask` and `ValueTask<T>` on .NET 11, in the sequential and hierarchical API styles, through the async detour support of MonoMod.Core 1.4. New tests prove interception from normal and runtime-async callers, and for original calls that are suspended when the mock is installed.
- Add the SMock Tests Claude Code plugin and usage references.
- Release SMock to NuGet, GitHub Packages, and GitHub Releases when a version tag is pushed, with changelog notes and package checksums.

### Improved

- Update MonoMod to `MonoMod.Core 1.4.0-prerelease.3` for every target. The .NET Framework targets now use the same `DetourFactory` hook code as the other targets, and the `MonoMod.RuntimeDetour` dependency is removed.
- Update the test packages: NUnit 5.0.0, NUnit3TestAdapter 6.3.1, Microsoft.NET.Test.Sdk 18.12.0, Microsoft.Extensions.Configuration.Binder and Json 11.0.0, and BenchmarkDotNet 0.15.8.
- Migrate the tests to NUnit 5: the async throws assertions are awaited, and the hierarchical tests keep the validation synchronous.
- Modernize the code with C# 14/15 features (primary constructors, collection expressions, `is null` patterns, switch expressions, file-scoped namespaces) for all targets, and use .NET 11 APIs only in the `net11.0` build. The hook code is logically unchanged, and there is no public API change.
- Remove .NET 8 and .NET 9 from the test targets, because both are out of Microsoft support. The tests now run on .NET 10, .NET 11 and .NET Framework 4.6.2 to 4.8.1.
- Update the CI workflows for the .NET 10 and .NET 11 SDKs, and add pull request checks for the documentation build and the release metadata.
- Update the documentation for .NET 10 and .NET 11, the prerelease install commands and the 2.7.0-preview.1 migration notes.
- Clarify context-based argument matching, advanced patterns, and troubleshooting documentation.
- Refresh CI actions and use the Intel macOS runner for x86_64 tests.

### Fixed

- Correct the hierarchical async example in the documentation: the validation `Action` must be synchronous and must finish while the hook is active.
- Include the license and README in the NuGet package using the correct paths.
- Fail the release gate when any Unix test framework fails.
- Replace the manual publishing workflows with a single tested tag-based release pipeline.

### Known limitations

- On the .NET 11 runtime, SMock cannot mock generic method instantiations that return `Task`, `Task<T>`, `ValueTask` or `ValueTask<T>` (for example `Task.FromResult<int>`), because MonoMod.Core 1.4.0-prerelease.3 does not support them yet. SMock throws a `NotSupportedException` with a clear message and keeps the MonoMod exception as `InnerException`. Non-generic async methods and all other target frameworks work.

## v2.6.0 - 2025-11-27

### New Features

- Add .NET 10 support (#45).

### Improved

- Update GitHub Actions dependencies (#41, #42, #43, #44, #46).

## v2.5.0 - 2025-08-18

### New Features

- Add ARM support (#39).

### Improved

- Update checkout, .NET setup, and artifact download actions (#34, #35, #37, #38).
- Update project funding links.

## v2.4.0 - 2025-03-21

### New Features

- Add native detour support (#32).

### Improved

- Update the test reporter action from v1 to v2 (#31).

## v2.3.0 - 2024-11-20

### New Features

- Add .NET 9 support (#28).

## v2.2.0 - 2024-02-08

### New Features

- Add .NET 8 support (#26).

### Improved

- Update documentation, NuGet badges, package website metadata, and README packaging.
- Enable code analysis and address compiler warnings.
- Update checkout, .NET setup, and artifact upload/download actions (#22–#25).

## v2.1.0 - 2023-08-29

### New Features

- Add disposable sequential mock setups alongside the hierarchical API (#20).

### Fixed

- Correct the build and package publishing pipelines.

## v2.0.0 - 2023-07-16

### Improved

- Update the core hooking engines (#17).
- Add generated API documentation, an introduction, and a GitHub Pages publishing workflow.
- Add project funding configuration and correct the README.

### Fixed

- Correct documentation paths and deployment configuration.
- Fix the package publishing pipeline.

## v1.8.0 - 2022-12-13

### New Features

- Add parameterized function mocking (#12).
- Add GitHub Packages publishing and a combined publishing workflow.

### Improved

- Configure Dependabot and update checkout, .NET setup, and artifact actions (#13–#16).
- Update license copyright information.

## v1.7.1 - 2022-08-27

### Fixed

- Remove debug logging from the `It` argument matcher implementation.

### Improved

- Update copyright information.

## v1.7.0 - 2022-08-27

### New Features

- Add setup contexts and context-based argument matching (#9).

### Improved

- Update README usage examples.

## v1.6.0 - 2021-12-21

### New Features

- Support throwing exceptions that do not have a default constructor (#7).

### Improved

- Update tests to .NET 6, refresh NuGet dependencies, and adopt newer C# language features.
- Add code ownership configuration.

### Fixed

- Correct NuGet publishing commands and the package source configuration.

## v1.5.0 - 2021-08-02

### New Features

- Add generic mock setup support (#6).

### Improved

- Make the internal `MethodMemoryInfoX32` implementation non-public.

## v1.4.0 - 2021-07-18

### New Features

- Add mocking for private methods and properties (#3).
- Add asynchronous return helpers (#5).

### Improved

- Add automated build and test workflows, including macOS testing in Release configuration.
- Update ignored development files.

### Fixed

- Correct task expectation tests for reflection-based setups.
- Fix solution paths and build/test configuration in CI.

## v1.3.0 - 2021-07-01

### Fixed

- Correct nested mocks of different functions when using `Returns` or `Throws` (#2).

## v1.2.1 - 2021-05-17

### Fixed

- Fix nested mocking and add regression coverage.
- Remove the experimental `It` implementation introduced during this development cycle.

## v1.2.0 - 2021-05-05

### New Features

- Add property mocking and accompanying tests.

### Improved

- Update the package description and README.

## v1.1.0 - 2021-04-30

### New Features

- Add callback functionality and type definitions for generic setups.

### Improved

- Add tests for instance methods, asynchronous static/instance methods, and callbacks.
- Correct package tags and add copyright metadata.

## v1.0.0 - 2021-04-27

### New Features

- Introduce static method mocking with runtime code injection.
- Add scoped mock execution, configured return values, exception throwing, and default void-method setups.
- Add initial mock behavior tests and x86/x64 debug configurations.
- Configure the SMock NuGet package, publishing scripts, and README.

### Fixed

- Stabilize exception setups and correct package version metadata.

## Release comparisons

- [Unreleased](https://github.com/svtlv/static-mock/compare/v2.7.0-preview.1...HEAD)
- [v2.7.0-preview.1](https://github.com/svtlv/static-mock/compare/v2.6.0...v2.7.0-preview.1)
- [v2.6.0](https://github.com/svtlv/static-mock/compare/v2.5.0...v2.6.0)
- [v2.5.0](https://github.com/svtlv/static-mock/compare/v2.4.0...v2.5.0)
- [v2.4.0](https://github.com/svtlv/static-mock/compare/v2.3.0...v2.4.0)
- [v2.3.0](https://github.com/svtlv/static-mock/compare/v2.2.0...v2.3.0)
- [v2.2.0](https://github.com/svtlv/static-mock/compare/v2.1.0...v2.2.0)
- [v2.1.0](https://github.com/svtlv/static-mock/compare/v2.0.0...v2.1.0)
- [v2.0.0](https://github.com/svtlv/static-mock/compare/v1.8.0...v2.0.0)
- [v1.8.0](https://github.com/svtlv/static-mock/compare/v1.7.1...v1.8.0)
- [v1.7.1](https://github.com/svtlv/static-mock/compare/v1.7.0...v1.7.1)
- [v1.7.0](https://github.com/svtlv/static-mock/compare/v1.6.0...v1.7.0)
- [v1.6.0](https://github.com/svtlv/static-mock/compare/v1.5.0...v1.6.0)
- [v1.5.0](https://github.com/svtlv/static-mock/compare/v1.4.0...v1.5.0)
- [v1.4.0](https://github.com/svtlv/static-mock/compare/v1.3.0...v1.4.0)
- [v1.3.0](https://github.com/svtlv/static-mock/compare/v1.2.1...v1.3.0)
- [v1.2.1](https://github.com/svtlv/static-mock/compare/v1.2.0...v1.2.1)
- [v1.2.0](https://github.com/svtlv/static-mock/compare/v1.1.0...v1.2.0)
- [v1.1.0](https://github.com/svtlv/static-mock/compare/v1.0.0...v1.1.0)
- [v1.0.0](https://github.com/svtlv/static-mock/tree/v1.0.0)
