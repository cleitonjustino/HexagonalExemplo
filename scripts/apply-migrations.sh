#!/usr/bin/env bash
# Applies FluentMigrator migrations by running the API project with --migrate-only
PROJECT=src/HexagonalExemplo.API
echo "Building and running migrations (bash)..."
cd "$(dirname "$0")/.."
dotnet run --project "$PROJECT" -- --migrate-only
