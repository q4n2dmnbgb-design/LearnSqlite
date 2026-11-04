param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$HtmlPath = "",
    [string]$PdfPath = "",
    [string]$ReportPath = ""
)

$ErrorActionPreference = "Stop"
$scriptPath = Join-Path $PSScriptRoot "Validate-Learning-Book.py"
if (-not (Test-Path $scriptPath)) {
    $scriptPath = Join-Path $ProjectRoot ".agents\skills\maui-step-guide-author\scripts\validate_learning_book.py"
}
if (-not (Test-Path $scriptPath)) {
    Write-Error "Could not find validate_learning_book.py."
    exit 1
}

if (-not $HtmlPath) { $HtmlPath = Join-Path $ProjectRoot "Learning\MAUI-Learning-Book.html" }
if (-not $PdfPath) { $PdfPath = Join-Path $ProjectRoot "Learning\MAUI-Learning-Book.pdf" }
if (-not $ReportPath) { $ReportPath = Join-Path $ProjectRoot "Learning\MAUI-Learning-Book.validation.json" }

python $scriptPath --html $HtmlPath --pdf $PdfPath --report $ReportPath
exit $LASTEXITCODE
