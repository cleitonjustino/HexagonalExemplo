using FluentAssertions;
using HexagonalExemplo.Dominio.Entidades;
using HexagonalExemplo.Dominio.ObjetosDeValor;
using Xunit;

namespace HexagonalExemplo.Testes.Dominio.Entidades;

public class TarefaTestes
{
    [Fact]
    public void Construtor_ComDadosValidos_DeveCriarTarefaPendente()
    {
        // Arrange
        var titulo = "Teste";
        var descricao = "Descrição teste";

        // Act
        var tarefa = new Tarefa(titulo, descricao);

        // Assert
        tarefa.Titulo.Should().Be(titulo);
        tarefa.Descricao.Should().Be(descricao);
        tarefa.Status.Should().Be(StatusTarefa.Pendente);
        tarefa.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        tarefa.DataConclusao.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    public void Construtor_ComTituloInvalido_DeveLancarExcecao(string tituloInvalido)
    {
        // Arrange
        var descricao = "Descrição válida";

        // Act
        Action act = () => new Tarefa(tituloInvalido, descricao);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Concluir_QuandoPendente_DeveMudarStatusEDataConclusao()
    {
        // Arrange
        var tarefa = new Tarefa("Teste", "Descrição");

        // Act
        tarefa.Concluir();

        // Assert
        tarefa.Status.Should().Be(StatusTarefa.Concluida);
        tarefa.DataConclusao.Should().NotBeNull();
        tarefa.DataConclusao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Concluir_QuandoJaConcluida_DeveLancarExcecao()
    {
        // Arrange
        var tarefa = new Tarefa("Teste", "Descrição");
        tarefa.Concluir();

        // Act
        Action act = () => tarefa.Concluir();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cancelar_QuandoPendente_DeveMudarStatus()
    {
        // Arrange
        var tarefa = new Tarefa("Teste", "Descrição");

        // Act
        tarefa.Cancelar();

        // Assert
        tarefa.Status.Should().Be(StatusTarefa.Cancelada);
    }

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAtualizarPropriedades()
    {
        // Arrange
        var tarefa = new Tarefa("Título Antigo", "Descrição Antiga");
        var novoTitulo = "Novo Título";
        var novaDescricao = "Nova Descrição";

        // Act
        tarefa.Atualizar(novoTitulo, novaDescricao);

        // Assert
        tarefa.Titulo.Should().Be(novoTitulo);
        tarefa.Descricao.Should().Be(novaDescricao);
    }

    [Fact]
    public void Atualizar_QuandoConcluida_DeveLancarExcecao()
    {
        // Arrange
        var tarefa = new Tarefa("Teste", "Descrição");
        tarefa.Concluir();

        // Act
        Action act = () => tarefa.Atualizar("Novo", "Novo");

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
}
