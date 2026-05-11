using HexagonalExemplo.Aplicacao.Interfaces;
using HexagonalExemplo.Dominio.Excecoes;
using HexagonalExemplo.Dominio.Repositorios;

namespace HexagonalExemplo.Aplicacao.CasosDeUso.Tarefas;

public class RemoverTarefaCasoDeUso : IRemoverTarefaCasoDeUso
{
    private readonly ITarefaRepositorio _tarefaRepositorio;

    public RemoverTarefaCasoDeUso(ITarefaRepositorio tarefaRepositorio)
    {
        _tarefaRepositorio = tarefaRepositorio ?? throw new ArgumentNullException(nameof(tarefaRepositorio));
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRepositorio.ObterPorIdAsync(id, cancellationToken)
            ?? throw new TarefaNaoEncontradaExcecao(id);

        await _tarefaRepositorio.RemoverAsync(id, cancellationToken);
    }
}
