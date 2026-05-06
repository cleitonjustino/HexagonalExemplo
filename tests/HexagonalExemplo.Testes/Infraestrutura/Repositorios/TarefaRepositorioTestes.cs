using FluentAssertions;
using HexagonalExemplo.Dominio.Entidades;
using HexagonalExemplo.Dominio.ObjetosDeValor;
using HexagonalExemplo.Infraestrutura.Persistencia;
using HexagonalExemplo.Infraestrutura.Repositorios;
using Xunit;

namespace HexagonalExemplo.Testes.Infraestrutura.Repositorios;

public class TarefaRepositorioTestes : IDisposable
{
    private readonly ConexaoDapper _conexaoDapper;
    private readonly TarefaRepositorio _repositorio;

    public TarefaRepositorioTestes()
    {
        // SQLite em memória com Cache=Shared para permitir múltiplas conexões no mesmo banco
        var connectionString = "Data Source=:memory:;Mode=Memory;Cache=Shared";
        _conexaoDapper = new ConexaoDapper(connectionString);
        
        // Criar tabelas usando o inicializador
        InicializadorBancoDados.Inicializar(_conexaoDapper);
        
        _repositorio = new TarefaRepositorio(_conexaoDapper);
    }

    public void Dispose()
    {
        // SQLite em memória é automaticamente liberado quando a última conexão fecha
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
