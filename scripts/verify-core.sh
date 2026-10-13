#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

echo "== SASD Editor Toolkit: Core verification =="
dotnet --version
dotnet restore tests/Sasd.EditorToolkit.Core.Tests/Sasd.EditorToolkit.Core.Tests.csproj
dotnet build tests/Sasd.EditorToolkit.Core.Tests/Sasd.EditorToolkit.Core.Tests.csproj --configuration Debug --no-restore
dotnet test tests/Sasd.EditorToolkit.Core.Tests/Sasd.EditorToolkit.Core.Tests.csproj --configuration Debug --no-build
git diff --check
echo "Core verification completed successfully."
