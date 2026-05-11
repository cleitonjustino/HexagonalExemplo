using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

var rootCommand = new RootCommand("Migration runner for HexagonalExemplo")
{
    new Option<string>(new[]{"--connection-string","-c"}, () => "Data Source=:memory:;Mode=Memory;Cache=Shared", "SQLite connection string")
};

rootCommand.SetHandler<string>(connectionString =>
{
    var services = new ServiceCollection()
        .AddFluentMigratorCore()
        .ConfigureRunner(rb => rb
            .AddSQLite()
            .WithGlobalConnectionString(connectionString)
            .ScanIn(typeof(HexagonalExemplo.Infraestrutura.Migrations._001_CreateTarefas).Assembly).For.Migrations())
        .AddLogging(lb => lb.AddFluentMigratorConsole());

    using var provider = services.BuildServiceProvider(false);
    using var scope = provider.CreateScope();

    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
    Console.WriteLine("Migrations applied.");
}, rootCommand.Options.First());

return await rootCommand.InvokeAsync(args);
