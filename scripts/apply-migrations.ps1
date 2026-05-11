# Applies FluentMigrator migrations by running the API project with --migrate-only
$project = "src/HexagonalExemplo.API"
Write-Host "Building and running migrations (PowerShell)..."
cd $PSScriptRoot/..\
dotnet run --project $project -- --migrate-only
