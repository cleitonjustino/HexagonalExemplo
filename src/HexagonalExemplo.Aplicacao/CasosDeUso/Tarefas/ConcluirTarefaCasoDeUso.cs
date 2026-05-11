using HexagonalExemplo.Aplicacao.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using HexagonalExemplo.Dominio.BarramentoDeMensagens;
using HexagonalExemplo.Dominio.Eventos;
using HexagonalExemplo.Dominio.Excecoes;
using HexagonalExemplo.Dominio.Repositorios;

namespace HexagonalExemplo.Aplicacao.CasosDeUso.Tarefas;

public class ConcluirTarefaCasoDeUso : IConcluirTarefaCasoDeUso
{
    private readonly ITarefaRepositorio _tarefaRepositorio;
    private readonly IServiceProvider _serviceProvider;

    public ConcluirTarefaCasoDeUso(ITarefaRepositorio tarefaRepositorio, IServiceProvider serviceProvider)
    {
        _tarefaRepositorio = tarefaRepositorio ?? throw new ArgumentNullException(nameof(tarefaRepositorio));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task ConcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRepositorio.ObterPorIdAsync(id, cancellationToken)
            ?? throw new TarefaNaoEncontradaExcecao(id);

        tarefa.Concluir();
        await _tarefaRepositorio.AtualizarAsync(tarefa, cancellationToken);

        var evento = TarefaConcluidaEvento.Criar(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.DataCriacao,
            tarefa.DataConclusao!.Value);

        var barramento = _serviceProvider.GetRequiredService<IBarramentoDeMensagens>();
        await barramento.PublicarAsync(evento, cancellationToken);
    }
}
