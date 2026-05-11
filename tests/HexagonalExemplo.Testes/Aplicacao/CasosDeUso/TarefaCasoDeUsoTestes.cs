using FluentAssertions;
using FluentValidation;
using HexagonalExemplo.Aplicacao.CasosDeUso.Tarefas;
using HexagonalExemplo.Aplicacao.DTOs;
using HexagonalExemplo.Aplicacao.Validadores;
using HexagonalExemplo.Dominio.BarramentoDeMensagens;
using HexagonalExemplo.Dominio.Entidades;
using HexagonalExemplo.Dominio.Repositorios;
using Moq;
using Xunit;

namespace HexagonalExemplo.Testes.Aplicacao.CasosDeUso;

public class TarefaCasoDeUsoTestes
{
    private readonly Mock<ITarefaRepositorio> _repositorioMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IBarramentoDeMensagens> _barramentoMock;
    private readonly IValidator<CriarTarefaRequest> _criarValidador;
    private readonly IValidator<AtualizarTarefaRequest> _atualizarValidador;
    private readonly CriarTarefaCasoDeUso _criarCasoDeUso;
    private readonly ObterTarefaPorIdCasoDeUso _obterPorIdCasoDeUso;
    private readonly ConcluirTarefaCasoDeUso _concluirCasoDeUso;
    private readonly ListarTarefasCasoDeUso _listarCasoDeUso;
    private readonly AtualizarTarefaCasoDeUso _atualizarCasoDeUso;
    private readonly RemoverTarefaCasoDeUso _removerCasoDeUso;

    public TarefaCasoDeUsoTestes()
    {
        _repositorioMock = new Mock<ITarefaRepositorio>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _barramentoMock = new Mock<IBarramentoDeMensagens>();
        
        _criarValidador = new CriarTarefaRequestValidador();
        _atualizarValidador = new AtualizarTarefaRequestValidador();
        
        _criarCasoDeUso = new CriarTarefaCasoDeUso(_repositorioMock.Object, _criarValidador);
        _obterPorIdCasoDeUso = new ObterTarefaPorIdCasoDeUso(_repositorioMock.Object);
        _concluirCasoDeUso = new ConcluirTarefaCasoDeUso(_repositorioMock.Object, _serviceProviderMock.Object);
        _listarCasoDeUso = new ListarTarefasCasoDeUso(_repositorioMock.Object);
        _atualizarCasoDeUso = new AtualizarTarefaCasoDeUso(_repositorioMock.Object, _atualizarValidador);
        _removerCasoDeUso = new RemoverTarefaCasoDeUso(_repositorioMock.Object);

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IBarramentoDeMensagens)))
            .Returns(_barramentoMock.Object);
    }

    [Fact]
    public async Task CriarAsync_ComDadosValidos_DeveCriarTarefa()
    {
        // Arrange
        var request = new CriarTarefaRequest("Nova Tarefa", "Descrição");
        Tarefa? tarefaCapturada = null;
        
        _repositorioMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Tarefa>(), It.IsAny<CancellationToken>()))
            .Callback<Tarefa, CancellationToken>((t, _) => tarefaCapturada = t)
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _criarCasoDeUso.CriarAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Titulo.Should().Be(request.Titulo);
        resultado.Descricao.Should().Be(request.Descricao);
        
        _repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Tarefa>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComTituloVazio_DeveLancarExcecaoValidacao()
    {
        // Arrange
        var request = new CriarTarefaRequest("", "Descrição");

        // Act
        Func<Task> act = () => _criarCasoDeUso.CriarAsync(request);

        // Assert
        await act.Should().ThrowAsync<HexagonalExemplo.Dominio.Excecoes.ValidacaoExcecao>();
        _repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Tarefa>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoExiste_DeveRetornarTarefa()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tarefa = new Tarefa("Teste", "Descrição");
        typeof(Tarefa).GetProperty("Id")?.SetValue(tarefa, id);
        
        _repositorioMock
            .Setup(r => r.ObterPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tarefa);

        // Act
        var resultado = await _obterPorIdCasoDeUso.ObterPorIdAsync(id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(id);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoNaoExiste_DeveRetornarNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        _repositorioMock
            .Setup(r => r.ObterPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tarefa?)null);

        // Act
        var resultado = await _obterPorIdCasoDeUso.ObterPorIdAsync(id);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ConcluirAsync_QuandoExiste_DeveConcluirEPublicarEvento()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tarefa = new Tarefa("Teste", "Descrição");
        typeof(Tarefa).GetProperty("Id")?.SetValue(tarefa, id);
        
        _repositorioMock
            .Setup(r => r.ObterPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tarefa);
        
        _repositorioMock
            .Setup(r => r.AtualizarAsync(It.IsAny<Tarefa>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _concluirCasoDeUso.ConcluirAsync(id);

        // Assert
        tarefa.Status.Should().Be(HexagonalExemplo.Dominio.ObjetosDeValor.StatusTarefa.Concluida);
        _repositorioMock.Verify(r => r.AtualizarAsync(tarefa, It.IsAny<CancellationToken>()), Times.Once);
        _barramentoMock.Verify(b => b.PublicarAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListarTodasAsync_DeveRetornarTodasTarefas()
    {
        // Arrange
        var tarefas = new[]
        {
            new Tarefa("Tarefa 1", "Desc 1"),
            new Tarefa("Tarefa 2", "Desc 2"),
            new Tarefa("Tarefa 3", "Desc 3")
        };
        
        _repositorioMock
            .Setup(r => r.ListarTodasAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tarefas);

        // Act
        var resultado = await _listarCasoDeUso.ListarTodasAsync();

        // Assert
        resultado.Should().HaveCount(3);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoExiste_DeveAtualizarTarefa()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tarefa = new Tarefa("Título Antigo", "Desc Antiga");
        typeof(Tarefa).GetProperty("Id")?.SetValue(tarefa, id);
        
        var request = new AtualizarTarefaRequest("Novo Título", "Nova Desc");
        
        _repositorioMock
            .Setup(r => r.ObterPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tarefa);
        
        _repositorioMock
            .Setup(r => r.AtualizarAsync(It.IsAny<Tarefa>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _atualizarCasoDeUso.AtualizarAsync(id, request);

        // Assert
        resultado.Titulo.Should().Be(request.Titulo);
        resultado.Descricao.Should().Be(request.Descricao);
        _repositorioMock.Verify(r => r.AtualizarAsync(tarefa, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_QuandoExiste_DeveChamarRepositorio()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tarefa = new Tarefa("Tarefa Teste", "Descrição");
        typeof(Tarefa).GetProperty("Id")?.SetValue(tarefa, id);
        
        _repositorioMock
            .Setup(r => r.ObterPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tarefa);
        
        _repositorioMock
            .Setup(r => r.RemoverAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _removerCasoDeUso.RemoverAsync(id);

        // Assert
        _repositorioMock.Verify(r => r.ObterPorIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositorioMock.Verify(r => r.RemoverAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
