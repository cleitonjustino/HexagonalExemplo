# Run the MigrateRunner tool via dotnet run
cd $PSScriptRoot/..\tools\MigrateRunner
Write-Host "Running MigrateRunner..."
dotnet run --project . -- -c "Data Source=:memory:;Mode=Memory;Cache=Shared"
