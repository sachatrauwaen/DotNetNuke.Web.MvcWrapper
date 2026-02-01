# Build script for SampleViewModule DNN install package
# Produces a zip suitable for Extensions -> Install Extension in DNN
# Uses MSBuild (required for .NET Framework 4.7.2 projects with PackageReference).

param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    [string]$OutDir = ""
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent (Split-Path -Parent $ScriptDir)
$SolutionPath = Join-Path $RepoRoot "Satrabel.Web.MvcWrapper.sln"
$StagingRoot = Join-Path $ScriptDir "obj\install-staging"
$ZipName = "SampleViewModule_install.zip"
$ZipDir = if ($OutDir) { $OutDir } else { $ScriptDir }
$ZipPath = Join-Path $ZipDir $ZipName

# Find MSBuild (dotnet build does not resolve packages correctly for this .NET Framework solution)
$MsBuild = $null
$Vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
if (Test-Path $Vswhere) {
    $MsBuild = & $Vswhere -latest -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" 2>$null | Select-Object -First 1
}
if (-not $MsBuild -or -not (Test-Path $MsBuild)) {
    $MsBuild = "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
    if (-not (Test-Path $MsBuild)) {
        $MsBuild = "${env:ProgramFiles}\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
    }
}
if (-not $MsBuild -or -not (Test-Path $MsBuild)) {
    throw "MSBuild not found. Install Visual Studio 2022 (or Build Tools) with .NET desktop build tools."
}

# Clean staging and obj (stale obj can cause RuntimeIdentifier restore errors)
if (Test-Path $StagingRoot) {
    Remove-Item -Recurse -Force $StagingRoot
}
New-Item -ItemType Directory -Path $StagingRoot -Force | Out-Null
$WrapperObj = Join-Path $RepoRoot "src\Satrabel.Web.MvcWrapper\obj"
if (Test-Path $WrapperObj) {
    Remove-Item -Recurse -Force $WrapperObj
}

# Restore then build (Restore must see RuntimeIdentifier in project file)
Write-Host "Restoring packages..." -ForegroundColor Cyan
& $MsBuild $SolutionPath /t:Restore /p:Configuration=$Configuration /v:minimal
if ($LASTEXITCODE -ne 0) { throw "Restore failed." }

Write-Host "Building solution ($Configuration)..." -ForegroundColor Cyan
& $MsBuild $SolutionPath /t:Build /p:Configuration=$Configuration /v:minimal
if ($LASTEXITCODE -ne 0) { throw "Build failed." }

# Paths to built outputs
$WrapperBin = Join-Path $RepoRoot "src\Satrabel.Web.MvcWrapper\bin\$Configuration"
$ModuleBin = Join-Path $ScriptDir "bin\$Configuration"

# Copy manifest
Copy-Item (Join-Path $ScriptDir "SampleViewModule.dnn") -Destination $StagingRoot

# Copy DLLs to staging/bin
$StagingBin = Join-Path $StagingRoot "bin"
New-Item -ItemType Directory -Path $StagingBin -Force | Out-Null
Copy-Item (Join-Path $ModuleBin "SampleViewModule.dll") -Destination $StagingBin
Copy-Item (Join-Path $WrapperBin "Satrabel.Web.MvcWrapper.dll") -Destination $StagingBin
if (Test-Path (Join-Path $ModuleBin "SampleViewModule.pdb")) {
    Copy-Item (Join-Path $ModuleBin "SampleViewModule.pdb") -Destination $StagingBin
}

# Copy Views to staging/DesktopModules/SampleViewModule/Views
$StagingViews = Join-Path $StagingRoot "DesktopModules\SampleViewModule\Views"
New-Item -ItemType Directory -Path $StagingViews -Force | Out-Null
Copy-Item (Join-Path $ScriptDir "Views\View.cshtml") -Destination $StagingViews
Copy-Item (Join-Path $ScriptDir "Views\web.config") -Destination $StagingViews

# Create zip
if (-not (Test-Path $ZipDir)) {
    New-Item -ItemType Directory -Path $ZipDir -Force | Out-Null
}
if (Test-Path $ZipPath) {
    Remove-Item $ZipPath -Force
}
Write-Host "Creating install package: $ZipPath" -ForegroundColor Cyan
Compress-Archive -Path "$StagingRoot\*" -DestinationPath $ZipPath -CompressionLevel Optimal

# Clean staging
Remove-Item -Recurse -Force $StagingRoot

Write-Host "Done. Install package: $ZipPath" -ForegroundColor Green
