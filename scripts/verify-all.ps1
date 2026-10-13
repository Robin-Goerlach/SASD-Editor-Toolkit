Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

Write-Host "== SASD Editor Toolkit: full Windows verification =="
dotnet --version
dotnet restore .\SASD-Editor-Toolkit.sln
dotnet build .\SASD-Editor-Toolkit.sln --configuration Debug --no-restore
dotnet test .\SASD-Editor-Toolkit.sln --configuration Debug --no-build
git diff --check
Write-Host "Full verification completed successfully."
