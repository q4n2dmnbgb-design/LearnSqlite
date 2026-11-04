param([string]$Root = (Split-Path -Parent $PSScriptRoot))

$ErrorActionPreference = "Stop"

$skills = @("maui-k12-baton-workflow", "maui-step-guide-author")
foreach ($skill in $skills) {
    New-Item -ItemType Directory -Path (Join-Path $Root ".agents\skills\$skill") -Force | Out-Null
}

$learningDirectories = @("Learning", "Learning\Guides", "Learning\Mockups", "Learning\Images")
foreach ($directory in $learningDirectories) {
    New-Item -ItemType Directory -Path (Join-Path $Root $directory) -Force | Out-Null
}
New-Item -ItemType Directory -Path (Join-Path $Root ".agents") -Force | Out-Null

function Write-FileIfMissing([string]$Path, [string]$Content) {
    if (-not (Test-Path $Path)) {
        $parent = Split-Path -Parent $Path
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
        Set-Content -LiteralPath $Path -Value $Content -Encoding UTF8
    }
}

Write-FileIfMissing (Join-Path $Root ".agents\MAUI-Agent-Mode.json") @'
{
  "studentCapabilityMode": true,
  "description": {
    "true": "MAXIMIZE STUDENT CAPABILITY",
    "false": "MAXIMIZE CODE GENERATED"
  }
}
'@

Write-FileIfMissing (Join-Path $Root "Learning\Current-Phase.md") @'
# Current Phase

Status: NOT_STARTED

Mode is controlled by `.agents/MAUI-Agent-Mode.json`.

Theory evidence:
Applied coding evidence:
Routine eligibility for agent-owned work:
'@

Write-FileIfMissing (Join-Path $Root "Learning\Student-Mastery.md") @'
# Student Mastery

Use these levels:
- Not introduced
- Practicing
- Understands
- Can work independently

For every concept, record:
- Theory evidence
- Applied coding evidence
- Evidence source (student-authored file/diff or reconstruction)
- Routine eligibility: Yes/No

Only mark `Can work independently` and `Routine eligibility: Yes` after both evidence tracks are demonstrated. Agent-generated or copied code never counts as applied coding evidence.
'@

Write-FileIfMissing (Join-Path $Root "Learning\MAUI-Learning-Book.md") @'
# MAUI Learning Book

This is the student's append-only learning book.
'@

function Sync-SkillIfPresent([string]$Source, [string]$Destination) {
    if (Test-Path $Source) {
        New-Item -ItemType Directory -Path $Destination -Force | Out-Null
        Copy-Item -Path (Join-Path $Source "*") -Destination $Destination -Recurse -Force
    }
}

# Support a local package checkout while keeping the nested companion skill canonical.
$packageRoot = Join-Path $Root "SkillPackages\maui-k12-baton-workflow"
if (Test-Path $packageRoot) {
    Sync-SkillIfPresent (Join-Path $packageRoot "skills\maui-k12-baton-workflow") (Join-Path $Root ".agents\skills\maui-k12-baton-workflow")
    Sync-SkillIfPresent (Join-Path $packageRoot "companion-skills\maui-step-guide-author\skills\maui-step-guide-author") (Join-Path $Root ".agents\skills\maui-step-guide-author")
}

$targetScriptsDir = Join-Path $Root "Scripts"
New-Item -ItemType Directory -Path $targetScriptsDir -Force | Out-Null
$skillScriptsDir = Join-Path $Root ".agents\skills\maui-step-guide-author\scripts"
$toolMap = @{
    "generate_book_pdf.py" = "Generate-Learning-Book-Pdf.py"
    "capture_mockup_blocks.py" = "Capture-Mockup-Blocks.py"
    "validate_learning_book.py" = "Validate-Learning-Book.py"
    "requirements-learning-book.txt" = "Learning-Book-Requirements.txt"
}

foreach ($entry in $toolMap.GetEnumerator()) {
    $source = Join-Path $skillScriptsDir $entry.Key
    if (Test-Path $source) {
        Copy-Item -LiteralPath $source -Destination (Join-Path $targetScriptsDir $entry.Value) -Force
    }
}

$learningBookSetup = Join-Path $targetScriptsDir "Setup-Learning-Book.ps1"
if (Test-Path $learningBookSetup) {
    & $learningBookSetup -ProjectRoot $Root
}

Write-Host "MAUI K12 skill structure, visual-capture tools, and learning-book validation are ready." -ForegroundColor Green
Write-Host "Default mode: MAXIMIZE STUDENT CAPABILITY" -ForegroundColor Cyan
