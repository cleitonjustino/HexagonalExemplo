using HexagonalExemplo.Aplicacao.DTOs;

namespace HexagonalExemplo.Aplicacao.Interfaces;

public interface IObterTarefaPorIdCasoDeUso
{
    Task<TarefaResponse?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
}
