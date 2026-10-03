[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $Tag,
    [string] $OutputPath
)

$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent

# NuGet versions: stable or prerelease SemVer, without build metadata.
if ($Tag -cnotmatch '^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)(?:-((?:0|[1-9][0-9]*|[0-9]*[A-Za-z-][0-9A-Za-z-]*)(?:\.(?:0|[1-9][0-9]*|[0-9]*[A-Za-z-][0-9A-Za-z-]*))*))?$') {
    throw "Invalid release tag '$Tag'. Use vX.Y.Z or vX.Y.Z-preview.N."
}
$version = $Tag.Substring(1)
[xml] $project = Get-Content (Join-Path $root 'src/StaticMock/StaticMock.csproj') -Raw
$projectVersion = $project.SelectSingleNode('/Project/PropertyGroup/Version').InnerText.Trim()
if ($projectVersion -cne $version) {
    throw "Tag version '$version' does not match the project Version '$projectVersion'."
}

$changelog = Get-Content (Join-Path $root 'CHANGELOG.md') -Raw
$heading = '(?m)^## ' + [regex]::Escape($Tag) + ' - \d{4}-\d{2}-\d{2}\r?$'
$sections = [regex]::Matches($changelog, $heading)
if ($sections.Count -ne 1) {
    throw "CHANGELOG.md must contain exactly one '## $Tag - YYYY-MM-DD' section."
}
$body = $changelog.Substring($sections[0].Index + $sections[0].Length)
$notes = ([regex]::Split($body, '(?m)^## '))[0].Trim()
if ($notes -notmatch '(?m)^- \S') {
    throw "The changelog section for '$Tag' must contain release notes as bullet points."
}

if ($OutputPath) {
    $fullPath = [IO.Path]::GetFullPath($OutputPath)
    [IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($fullPath)) | Out-Null
    [IO.File]::WriteAllText($fullPath, $notes + "`n", [Text.UTF8Encoding]::new($false))
}
if ($env:GITHUB_OUTPUT) {
    "version=$version" >> $env:GITHUB_OUTPUT
    "prerelease=$($version.Contains('-').ToString().ToLowerInvariant())" >> $env:GITHUB_OUTPUT
}
Write-Host "Validated $Tag"
