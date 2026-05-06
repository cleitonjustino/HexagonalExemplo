using FluentValidation;
using HexagonalExemplo.Aplicacao.DTOs;
using HexagonalExemplo.Aplicacao.Interfaces;
using HexagonalExemplo.Aplicacao.Mapeamentos;
using HexagonalExemplo.Dominio.BarramentoDeMensagens;
using HexagonalExemplo.Dominio.Entidades;
using HexagonalExemplo.Dominio.Eventos;
using HexagonalExemplo.Dominio.Excecoes;
using HexagonalExemplo.Dominio.Repositorios;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace HexagonalExemplo.Aplicacao.CasosDeUso;

public class TarefaCasoDeUso : ITarefaCasoDeUso
{
    private readonly ITarefaRepositorio _tarefaRepositorio;
    private readonly IServiceProvider _serviceProvider;
    private readonly IValidator<CriarTarefaRequest> _criarValidador;
    private readonly IValidator<AtualizarTarefaRequest> _atualizarValidador;

    public TarefaCasoDeUso(
        ITarefaRepositorio tarefaRepositorio,
        IServiceProvider serviceProvider,
        IValidator<CriarTarefaRequest> criarValidador,
        IValidator<AtualizarTarefaRequest> atualizarValidador)
    {
        _tarefaRepositorio = tarefaRepositorio ?? throw new ArgumentNullException(nameof(tarefaRepositorio));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _criarValidador = criarValidador ?? throw new ArgumentNullException(nameof(criarValidador));
        _atualizarValidador = atualizarValidador ?? throw new ArgumentNullException(nameof(atualizarValidador));
    }

    public async Task<TarefaResponse> CriarAsync(CriarTarefaRequest request, CancellationToken cancellationToken = default)
    {
        var validacao = await _criarValidador.ValidateAsync(request, cancellationToken);
        if (!validacao.IsValid)
            throw new ValidacaoExcecao(string.Join(", ", validacao.Errors.Select(e => e.ErrorMessage)));

        var tarefa = new Tarefa(request.Titulo, request.Descricao);
        await _tarefaRepositorio.AdicionarAsync(tarefa, cancellationToken);
        return tarefa.ParaResponse();
    }

    public async Task<TarefaResponse?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRepositorio.ObterPorIdAsync(id, cancellationToken);
        return tarefa?.ParaResponse();
    }

    public async Task<IEnumerable<TarefaResponse>> ListarTodasAsync(CancellationToken cancellationToken = default)
    {
        var tarefas = await _tarefaRepositorio.ListarTodasAsync(cancellationToken);
        return tarefas.ParaResponseList();
    }

    public async Task<TarefaResponse> AtualizarAsync(Guid id, AtualizarTarefaRequest request, CancellationToken cancellationToken = default)
    {
        var validacao = await _atualizarValidador.ValidateAsync(request, cancellationToken);
        if (!validacao.IsValid)
            throw new ValidacaoExcecao(string.Join(", ", validacao.Errors.Select(e => e.ErrorMessage)));

        var tarefa = await _tarefaRepositorio.ObterPorIdAsync(id, cancellationToken)
            ?? throw new TarefaNaoEncontradaExcecao(id);

        tarefa.Atualizar(request.Titulo, request.Descricao);
        await _tarefaRepositorio.AtualizarAsync(tarefa, cancellationToken);

        return tarefa.ParaResponse();
    }

    public async Task ConcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRepositorio.ObterPorIdAsync(id, cancellationToken)
            ?? throw new TarefaNaoEncontradaExcecao(id);

        tarefa.Concluir();
        await _tarefaRepositorio.AtualizarAsync(tarefa, cancellationToken);

        // Publicar evento de domínio para notificar outros serviços
        var evento = TarefaConcluidaEvento.Criar(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.DataCriacao,
            tarefa.DataConclusao!.Value);

        var barramento = _serviceProvider.GetRequiredService<IBarramentoDeMensagens>();
        await barramento.PublicarAsync(evento, cancellationToken);
    }

    public async Task CancelarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRepositorio.ObterPorIdAsync(id, cancellationToken)
            ?? throw new TarefaNaoEncontradaExcecao(id);

        tarefa.Cancelar();
        await _tarefaRepositorio.AtualizarAsync(tarefa, cancellationToken);
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRepositorio.ObterPorIdAsync(id, cancellationToken)
            ?? throw new TarefaNaoEncontradaExcecao(id);

        await _tarefaRepositorio.RemoverAsync(id, cancellationToken);
    }
}
