using HexagonalExemplo.Aplicacao.DTOs;
using HexagonalExemplo.Aplicacao.Interfaces;
using HexagonalExemplo.Aplicacao.Mapeamentos;
using HexagonalExemplo.Dominio.Repositorios;

namespace HexagonalExemplo.Aplicacao.CasosDeUso.Tarefas;

public class ObterTarefaPorIdCasoDeUso : IObterTarefaPorIdCasoDeUso
{
    private readonly ITarefaRepositorio _tarefaRepositorio;

    public ObterTarefaPorIdCasoDeUso(ITarefaRepositorio tarefaRepositorio)
    {
        _tarefaRepositorio = tarefaRepositorio ?? throw new ArgumentNullException(nameof(tarefaRepositorio));
    }

    public async Task<TarefaResponse?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRepositorio.ObterPorIdAsync(id, cancellationToken);
        return tarefa?.ParaResponse();
    }
}
