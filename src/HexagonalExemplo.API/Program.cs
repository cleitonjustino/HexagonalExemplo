using FluentValidation;
using FluentValidation.AspNetCore;
using FluentMigrator.Runner;
using HexagonalExemplo.Aplicacao.CasosDeUso.Tarefas;
using HexagonalExemplo.Aplicacao.Interfaces;
using HexagonalExemplo.Infraestrutura.Migrations;
using HexagonalExemplo.Aplicacao.Validadores;
using HexagonalExemplo.Dominio.Repositorios;
using HexagonalExemplo.Infraestrutura.Persistencia;
using HexagonalExemplo.Infraestrutura.Repositorios;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();

// Configurar Dapper com SQLite em memória
var connectionString = "Data Source=:memory:;Mode=Memory;Cache=Shared";
builder.Services.AddSingleton<IConexaoDapper>(_ => new ConexaoDapper(connectionString));

// Injeção de dependências - Ports and Adapters
builder.Services.AddScoped<ITarefaRepositorio, TarefaRepositorio>();

// Registrar casos de uso individuais
builder.Services.AddScoped<IListarTarefasCasoDeUso, ListarTarefasCasoDeUso>();
builder.Services.AddScoped<IObterTarefaPorIdCasoDeUso, ObterTarefaPorIdCasoDeUso>();
builder.Services.AddScoped<ICriarTarefaCasoDeUso, CriarTarefaCasoDeUso>();
builder.Services.AddScoped<IAtualizarTarefaCasoDeUso, AtualizarTarefaCasoDeUso>();
builder.Services.AddScoped<IConcluirTarefaCasoDeUso, ConcluirTarefaCasoDeUso>();
builder.Services.AddScoped<ICancelarTarefaCasoDeUso, CancelarTarefaCasoDeUso>();
builder.Services.AddScoped<IRemoverTarefaCasoDeUso, RemoverTarefaCasoDeUso>();

// FluentMigrator - configurar runner para SQLite e apontar para assembly de migrations
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSQLite()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(_001_CreateTarefas).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

// FluentValidation - registrar todos os validadores do assembly
builder.Services.AddValidatorsFromAssemblyContaining<CriarTarefaRequestValidador>();

builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.AutomaticValidationEnabled = true;
        fv.DisableDataAnnotationsValidation = true;
    });

builder.Services.AddProblemDetails();

var app = builder.Build();

// Se a aplicação for executada com o argumento `--migrate-only`, aplica migrations e sai
if (args is not null && args.Contains("--migrate-only"))
{
    using (var scope = app.Services.CreateScope())
    {
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    Console.WriteLine("Migrations aplicadas (modo migrate-only).");
    return;
}

// Executar migrations e semear dados (comportamento padrão na inicialização)
using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();

    var conexaoDapper = scope.ServiceProvider.GetRequiredService<IConexaoDapper>();
    await SemearDadosAsync(conexaoDapper);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "HexagonalExemplo API";
        options.Theme = ScalarTheme.Purple;
        options.ShowSidebar = true;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Redirecionar raiz para Scalar
app.MapGet("/", () => Results.Redirect("/scalar"));

app.Run();
return;

async Task SemearDadosAsync(IConexaoDapper conexaoDapper)
{
    using var conexao = conexaoDapper.CriarConexao();
    var repositorio = new TarefaRepositorio(conexaoDapper);

    var tarefas = new[]
    {
        new HexagonalExemplo.Dominio.Entidades.Tarefa("Apresentar Arquitetura Hexagonal", "Preparar demo"),
        new HexagonalExemplo.Dominio.Entidades.Tarefa("Configurar Scalar", "Adicionar documentação da API"),
        new HexagonalExemplo.Dominio.Entidades.Tarefa("Implementar Ports", "Definir interfaces do domínio")
    };

    foreach (var tarefa in tarefas)
    {
        await repositorio.AdicionarAsync(tarefa);
    }
}
