using HexagonalExemplo.Aplicacao.Interfaces;
using HexagonalExemplo.Dominio.Excecoes;
using HexagonalExemplo.Dominio.Repositorios;

namespace HexagonalExemplo.Aplicacao.CasosDeUso;

public class CancelarTarefaCasoDeUso : ICancelarTarefaCasoDeUso
{
    private readonly ITarefaRepositorio _tarefaRepositorio;

    public CancelarTarefaCasoDeUso(ITarefaRepositorio tarefaRepositorio)
    {
        _tarefaRepositorio = tarefaRepositorio ?? throw new ArgumentNullException(nameof(tarefaRepositorio));
    }

    public async Task CancelarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRepositorio.ObterPorIdAsync(id, cancellationToken)
            ?? throw new TarefaNaoEncontradaExcecao(id);

        tarefa.Cancelar();
        await _tarefaRepositorio.AtualizarAsync(tarefa, cancellationToken);
    }
}
