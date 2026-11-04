# Setup-Learning-Book.ps1
# Checks the local Python and Chromium prerequisites for learning-book generation.

param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [switch]$InstallMissing
)

$ErrorActionPreference = "Stop"

function Get-PythonCommand {
    foreach ($candidate in @("python", "python3")) {
        $command = Get-Command $candidate -CommandType Application -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($command) {
            return $command.Source
        }
    }

    return $null
}

function Test-LearningBookPrerequisites {
    param([string]$PythonCommand)

    $probeScript = Join-Path $PSScriptRoot "Check-Learning-Book-Prerequisites.py"
    if (-not (Test-Path $probeScript)) {
        Write-Host "Missing learning-book prerequisite checker: $probeScript" -ForegroundColor Yellow
        return $false
    }

    $probeOutput = @(& $PythonCommand $probeScript 2>&1)
    $probeExitCode = $LASTEXITCODE

    foreach ($message in $probeOutput) {
        if ($message) {
            Write-Host $message -ForegroundColor Yellow
        }
    }

    return $probeExitCode -eq 0
}

$requirementsFile = Join-Path $ProjectRoot "Scripts\Learning-Book-Requirements.txt"
if (-not (Test-Path $requirementsFile)) {
    $requirementsFile = Join-Path $PSScriptRoot "Learning-Book-Requirements.txt"
}
$pythonCommand = Get-PythonCommand
$prerequisitesReady = $false

if ($pythonCommand) {
    $prerequisitesReady = Test-LearningBookPrerequisites -PythonCommand $pythonCommand
} else {
    Write-Host "Missing learning-book prerequisite: Python 3." -ForegroundColor Yellow
}

if ($prerequisitesReady) {
    Write-Host "Learning-book prerequisites are already available. No downloads were performed." -ForegroundColor Green
    return
}

Write-Host "Learning-book generation needs the declared Python packages and the Playwright Chromium runtime." -ForegroundColor Yellow
Write-Host "Installing them may download browser binaries, use disk space, and change the selected Python environment." -ForegroundColor Yellow

if (-not $InstallMissing) {
    Write-Host "No downloads were performed. Run .\\Scripts\\Setup-Learning-Book.ps1 -InstallMissing when you are ready." -ForegroundColor Cyan
    $global:LASTEXITCODE = 0
    return
}

if (-not $pythonCommand) {
    throw "Python 3 is required before learning-book dependencies can be installed."
}

if (-not (Test-Path $requirementsFile)) {
    throw "Cannot find the learning-book requirements file: $requirementsFile"
}

Write-Host "Installing missing learning-book prerequisites..." -ForegroundColor Cyan
& $pythonCommand -m pip install -r $requirementsFile
if ($LASTEXITCODE -ne 0) {
    throw "Python dependency installation failed with exit code $LASTEXITCODE."
}

& $pythonCommand -m playwright install chromium
if ($LASTEXITCODE -ne 0) {
    throw "Playwright Chromium installation failed with exit code $LASTEXITCODE."
}

if (-not (Test-LearningBookPrerequisites -PythonCommand $pythonCommand)) {
    throw "Learning-book prerequisites are still incomplete after installation."
}

Write-Host "Learning-book prerequisites are ready." -ForegroundColor Green
