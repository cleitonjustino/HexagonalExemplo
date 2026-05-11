#!/usr/bin/env bash
# Run the MigrateRunner tool via dotnet run
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$SCRIPT_DIR/../tools/MigrateRunner"
echo "Running MigrateRunner..."
dotnet run --project . -- -c "Data Source=:memory:;Mode=Memory;Cache=Shared"
