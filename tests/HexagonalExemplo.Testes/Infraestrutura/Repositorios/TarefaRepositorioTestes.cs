using FluentAssertions;
using HexagonalExemplo.Dominio.Entidades;
using HexagonalExemplo.Dominio.ObjetosDeValor;
using HexagonalExemplo.Infraestrutura.Persistencia;
using HexagonalExemplo.Infraestrutura.Repositorios;
using Microsoft.Data.Sqlite;
using Xunit;

namespace HexagonalExemplo.Testes.Infraestrutura.Repositorios;

public class TarefaRepositorioTestes : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ConexaoDapper _conexaoDapper;
    private readonly TarefaRepositorio _repositorio;

    public TarefaRepositorioTestes()
    {
        // Usar SQLite em arquivo para testes de integração
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        
        _conexaoDapper = new ConexaoDapper("Data Source=:memory:");
        // Criar tabela
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE Tarefas (
                Id TEXT PRIMARY KEY,
                Titulo TEXT NOT NULL,
                Descricao TEXT,
                Status INTEGER NOT NULL,
                DataCriacao TEXT NOT NULL,
                DataConclusao TEXT
            );";
        cmd.ExecuteNonQuery();
        
        _repositorio = new TarefaRepositorio(_conexaoDapper);
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }

    [Fact]
    public async Task AdicionarAsync_DeveInserirTarefaNoBanco()
    {
        // Arrange
        var tarefa = new Tarefa("Tarefa Teste", "Descrição teste");

        // Act
        await _repositorio.AdicionarAsync(tarefa);

        // Assert
        var tarefaDoBanco = await _repositorio.ObterPorIdAsync(tarefa.Id);
        tarefaDoBanco.Should().NotBeNull();
        tarefaDoBanco!.Titulo.Should().Be(tarefa.Titulo);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoExiste_DeveRetornarTarefa()
    {
        // Arrange
        var tarefa = new Tarefa("Tarefa Teste", "Descrição");
        await _repositorio.AdicionarAsync(tarefa);

        // Act
        var resultado = await _repositorio.ObterPorIdAsync(tarefa.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(tarefa.Id);
        resultado.Titulo.Should().Be(tarefa.Titulo);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoNaoExiste_DeveRetornarNull()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var resultado = await _repositorio.ObterPorIdAsync(idInexistente);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ListarTodasAsync_DeveRetornarTodasAsTarefas()
    {
        // Arrange
        await _repositorio.AdicionarAsync(new Tarefa("Tarefa 1", "Desc 1"));
        await _repositorio.AdicionarAsync(new Tarefa("Tarefa 2", "Desc 2"));
        await _repositorio.AdicionarAsync(new Tarefa("Tarefa 3", "Desc 3"));

        // Act
        var resultado = await _repositorio.ListarTodasAsync();

        // Assert
        resultado.Should().HaveCount(3);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarTarefaNoBanco()
    {
        // Arrange
        var tarefa = new Tarefa("Título Original", "Desc Original");
        await _repositorio.AdicionarAsync(tarefa);
        
        tarefa.Atualizar("Título Atualizado", "Desc Atualizada");

        // Act
        await _repositorio.AtualizarAsync(tarefa);

        // Assert
        var tarefaAtualizada = await _repositorio.ObterPorIdAsync(tarefa.Id);
        tarefaAtualizada!.Titulo.Should().Be("Título Atualizado");
        tarefaAtualizada.Descricao.Should().Be("Desc Atualizada");
    }

    [Fact]
    public async Task ListarPorStatusAsync_DeveRetornarApenasTarefasComStatusEspecifico()
    {
        // Arrange
        var tarefa1 = new Tarefa("Tarefa 1", "Desc 1");
        var tarefa2 = new Tarefa("Tarefa 2", "Desc 2");
        var tarefa3 = new Tarefa("Tarefa 3", "Desc 3");
        
        await _repositorio.AdicionarAsync(tarefa1);
        await _repositorio.AdicionarAsync(tarefa2);
        await _repositorio.AdicionarAsync(tarefa3);
        
        tarefa1.Concluir();
        await _repositorio.AtualizarAsync(tarefa1);

        // Act
        var concluidas = await _repositorio.ListarPorStatusAsync(StatusTarefa.Concluida);
        var pendentes = await _repositorio.ListarPorStatusAsync(StatusTarefa.Pendente);

        // Assert
        concluidas.Should().HaveCount(1);
        concluidas.First().Titulo.Should().Be("Tarefa 1");
        pendentes.Should().HaveCount(2);
    }

    [Fact]
    public async Task RemoverAsync_DeveExcluirTarefaDoBanco()
    {
        // Arrange
        var tarefa = new Tarefa("Tarefa Teste", "Descrição");
        await _repositorio.AdicionarAsync(tarefa);
        
        var tarefaNoBanco = await _repositorio.ObterPorIdAsync(tarefa.Id);
        tarefaNoBanco.Should().NotBeNull();

        // Act
        await _repositorio.RemoverAsync(tarefa.Id);

        // Assert
        var tarefaRemovida = await _repositorio.ObterPorIdAsync(tarefa.Id);
        tarefaRemovida.Should().BeNull();
    }
}
