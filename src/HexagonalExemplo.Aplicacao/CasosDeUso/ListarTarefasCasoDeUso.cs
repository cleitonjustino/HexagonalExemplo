using HexagonalExemplo.Aplicacao.DTOs;
using HexagonalExemplo.Aplicacao.Interfaces;
using HexagonalExemplo.Aplicacao.Mapeamentos;
using HexagonalExemplo.Dominio.Repositorios;

namespace HexagonalExemplo.Aplicacao.CasosDeUso;

public class ListarTarefasCasoDeUso : IListarTarefasCasoDeUso
{
    private readonly ITarefaRepositorio _tarefaRepositorio;

    public ListarTarefasCasoDeUso(ITarefaRepositorio tarefaRepositorio)
    {
        _tarefaRepositorio = tarefaRepositorio ?? throw new ArgumentNullException(nameof(tarefaRepositorio));
    }

    public async Task<IEnumerable<TarefaResponse>> ListarTodasAsync(CancellationToken cancellationToken = default)
    {
        var tarefas = await _tarefaRepositorio.ListarTodasAsync(cancellationToken);
        return tarefas.ParaResponseList();
    }
}
