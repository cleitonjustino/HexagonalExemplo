using HexagonalExemplo.Aplicacao.DTOs;

namespace HexagonalExemplo.Aplicacao.Interfaces;

public interface IAtualizarTarefaCasoDeUso
{
    Task<TarefaResponse> AtualizarAsync(Guid id, AtualizarTarefaRequest request, CancellationToken cancellationToken = default);
}
