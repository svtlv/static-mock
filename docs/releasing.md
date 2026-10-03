# Releasing SMock

Releases follow the Ralphex model: maintain a categorized `CHANGELOG.md`, commit
the release metadata, and push an annotated `vX.Y.Z` tag. GitHub Actions handles
the build and publication. SMock uses `dotnet pack` in place of GoReleaser.

## One-time setup

- Set the repository Actions secret `NUGET_API_KEY` to a NuGet.org API key with
  permission to push the SMock package.
- Allow GitHub Actions to publish the SMock GitHub package. If the package already
  exists, grant this repository Actions access in the package settings.
- The workflow grants its `GITHUB_TOKEN` release and package write permissions;
  no separate GitHub personal access token is needed.

## Prepare a release

1. Update `<Version>` in `src/StaticMock/StaticMock.csproj`.
2. Move the applicable `Unreleased` entries in `CHANGELOG.md` to a new section
   named `## vX.Y.Z - YYYY-MM-DD`. Keep the `New Features`, `Improved`, and `Fixed`
   categories where applicable, and leave an `Unreleased` section for future work.
3. Validate from the repository root (replace the example version throughout):

   ```powershell
   ./scripts/prepare-release.ps1 -Tag v2.7.0
   dotnet test src --configuration Release
   ```

   The full local test command requires Windows and .NET 8, 9, and 10 runtimes.
   CI runs Debug and Release tests on Windows, Linux, Intel macOS, and ARM macOS.
4. Commit the changes and merge them into `master`. From the updated, clean
   `master` checkout, create and push the tag:

   ```powershell
   git tag -a v2.7.0 -m "Version 2.7.0"
   git push origin v2.7.0
   ```

The tag, project version, and changelog heading must agree. Stable tags use
`vX.Y.Z`; prereleases use tags such as `v2.7.0-preview.1` and a project version of
`2.7.0-preview.1`. Build metadata (`+...`) is not supported. The Claude Code
plugin retains its independent version in `smock-plugin/`.

## What the workflow publishes

After metadata validation and the full test matrix pass, CI builds one SMock
NuGet package and records its SHA-256 checksum. The same artifact is published
to NuGet.org and GitHub Packages. A GitHub release then receives the package,
`checksums.txt`, and the matching changelog section as its release notes.
Prereleases are marked as such and are not marked as the latest stable release.

Watch the repository's **Actions → release** run. Publication is not atomic
across the two registries and GitHub Releases. If a later step fails, rerun the
failed jobs in that run; package pushes skip versions already published. Fix
missing or expired credentials before retrying. Never move a published tag or
reuse a published version for changed package contents.

The release workflow also supports manual dispatch against an existing version
tag (for example, `gh workflow run release.yaml --ref v2.7.0`); branch dispatches
are rejected. Prefer rerunning failed jobs to retain the original package
artifact. Do not dispatch an already completed release: its GitHub release
already exists. Code or changelog corrections after publication need a new version.
