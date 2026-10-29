# Changelog

Release history reconstructed from all repository version tags. Dates are the
tagged commits' dates; entries summarize the changes since the preceding tag.
Version-only commits and intermediate CI corrections are grouped with their
related changes. The Claude Code plugin has its own version; its changes after
the latest SMock tag are listed under `Unreleased`.

## Unreleased

### New Features

- Add the SMock Tests Claude Code plugin and usage references.
- Release SMock to NuGet, GitHub Packages, and GitHub Releases when a version tag is pushed, with changelog notes and package checksums.

### Improved

- Clarify context-based argument matching, advanced patterns, and troubleshooting documentation.
- Refresh CI actions and use the Intel macOS runner for x86_64 tests.

### Fixed

- Include the license and README in the NuGet package using the correct paths.
- Fail the release gate when any Unix test framework fails.
- Replace the manual publishing workflows with a single tested tag-based release pipeline.

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

- [Unreleased](https://github.com/svtlv/static-mock/compare/v2.6.0...HEAD)
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
