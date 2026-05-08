using HexagonalExemplo.Aplicacao.DTOs;

namespace HexagonalExemplo.Aplicacao.Interfaces;

public interface IListarTarefasCasoDeUso
{
    Task<IEnumerable<TarefaResponse>> ListarTodasAsync(CancellationToken cancellationToken = default);
}
