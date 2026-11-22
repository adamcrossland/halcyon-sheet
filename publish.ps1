<#
.SYNOPSIS
    Builds self-contained Halcyon packages for Windows and macOS.

.DESCRIPTION
    Each package is a zip with a single-file executable (no .NET install needed), its wwwroot folder
    and a READ ME. Output goes to artifacts\packages.

.EXAMPLE
    ./publish.ps1
    ./publish.ps1 -Targets windows
#>
param(
    [ValidateSet('windows', 'mac-apple-silicon', 'mac-intel')]
    [string[]] $Targets = @('windows', 'mac-apple-silicon', 'mac-intel'),
    [string] $Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression, System.IO.Compression.FileSystem

$root = $PSScriptRoot
$project = Join-Path $root 'src/Halcyon.Web/Halcyon.Web.csproj'
$staging = Join-Path $root 'artifacts/publish'
$packages = Join-Path $root 'artifacts/packages'

$rids = @{
    'windows'           = 'win-x64'
    'mac-apple-silicon' = 'osx-arm64'
    'mac-intel'         = 'osx-x64'
}

# Zip files made on Windows are marked "made by Windows", and macOS then ignores Unix permissions,
# so the app wouldn't be executable. Write each entry's Unix mode and mark the zip "made by Unix".
function New-UnixZip([string] $sourceDir, [string] $zipPath, [string] $topFolder, [string[]] $executables) {
    if (Test-Path $zipPath) { Remove-Item $zipPath }
    $zip = [System.IO.Compression.ZipFile]::Open($zipPath, 'Create')
    try {
        Get-ChildItem $sourceDir -Recurse -File | ForEach-Object {
            $relative = [System.IO.Path]::GetRelativePath($sourceDir, $_.FullName).Replace('\', '/')
            $entry = [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $_.FullName, "$topFolder/$relative", 'Optimal')
            $mode = if ($executables -contains $relative) { 0x81ED } else { 0x81A4 }   # 100755 / 100644
            $entry.ExternalAttributes = $mode -shl 16
        }
    }
    finally { $zip.Dispose() }

    # Set the "version made by" OS byte of every central-directory record to 3 (Unix).
    $bytes = [System.IO.File]::ReadAllBytes($zipPath)
    $eocd = -1
    for ($i = $bytes.Length - 22; $i -ge 0; $i--) {
        if ([BitConverter]::ToUInt32($bytes, $i) -eq 0x06054b50) { $eocd = $i; break }
    }
    if ($eocd -lt 0) { throw "No end-of-central-directory record in $zipPath" }
    $count = [BitConverter]::ToUInt16($bytes, $eocd + 10)
    $offset = [BitConverter]::ToUInt32($bytes, $eocd + 16)
    for ($n = 0; $n -lt $count; $n++) {
        if ([BitConverter]::ToUInt32($bytes, $offset) -ne 0x02014b50) { throw "Bad central directory in $zipPath" }
        $bytes[$offset + 5] = 3
        $nameLen = [BitConverter]::ToUInt16($bytes, $offset + 28)
        $extraLen = [BitConverter]::ToUInt16($bytes, $offset + 30)
        $commentLen = [BitConverter]::ToUInt16($bytes, $offset + 32)
        $offset += 46 + $nameLen + $extraLen + $commentLen
    }
    [System.IO.File]::WriteAllBytes($zipPath, $bytes)
}

New-Item -ItemType Directory -Force $packages | Out-Null

foreach ($target in $Targets) {
    $rid = $rids[$target]
    $out = Join-Path $staging $target
    Write-Host "`n=== $target ($rid) ===" -ForegroundColor Cyan
    if (Test-Path $out) { Remove-Item -Recurse -Force $out }

    dotnet publish $project -c $Configuration -r $rid --self-contained true -o $out `
        -p:PublishSingleFile=true `
        -p:EnableCompressionInSingleFile=true `
        -p:IncludeNativeLibrariesForSelfExtract=true `
        -p:DebugType=None `
        -p:GenerateDocumentationFile=false
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed for $rid" }

    # Tidy: development settings and IIS config aren't needed by players.
    Remove-Item -ErrorAction SilentlyContinue (Join-Path $out 'appsettings.Development.json'), (Join-Path $out 'web.config')
    Get-ChildItem $out -Filter *.pdb | Remove-Item

    $forWindows = $rid.StartsWith('win')
    $exeFrom = Join-Path $out ($(if ($forWindows) { 'Halcyon.Web.exe' } else { 'Halcyon.Web' }))
    $exeName = if ($forWindows) { 'Halcyon.exe' } else { 'Halcyon' }
    Move-Item $exeFrom (Join-Path $out $exeName) -Force

    if ($forWindows) {
        Copy-Item (Join-Path $root 'packaging/windows/*') $out
    }
    else {
        Copy-Item (Join-Path $root 'packaging/mac/READ ME.txt') $out
        # The launcher must have Unix line endings to run.
        $cmd = [System.IO.File]::ReadAllText((Join-Path $root 'packaging/mac/Start Halcyon.command')).Replace("`r`n", "`n")
        [System.IO.File]::WriteAllText((Join-Path $out 'Start Halcyon.command'), $cmd, [System.Text.UTF8Encoding]::new($false))
    }

    $zipPath = Join-Path $packages "Halcyon-$target.zip"
    New-UnixZip $out $zipPath 'Halcyon' @('Halcyon', 'Start Halcyon.command')
    $sizeMb = [math]::Round((Get-Item $zipPath).Length / 1MB, 1)
    Write-Host "Created $zipPath ($sizeMb MB)" -ForegroundColor Green
}

Write-Host "`nPackages are in $packages"
