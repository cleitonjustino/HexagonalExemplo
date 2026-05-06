using HexagonalExemplo.Aplicacao.DTOs;

namespace HexagonalExemplo.Aplicacao.Interfaces;

public interface ITarefaCasoDeUso
{
    Task<TarefaResponse> CriarAsync(CriarTarefaRequest request, CancellationToken cancellationToken = default);
    Task<TarefaResponse?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TarefaResponse>> ListarTodasAsync(CancellationToken cancellationToken = default);
    Task<TarefaResponse> AtualizarAsync(Guid id, AtualizarTarefaRequest request, CancellationToken cancellationToken = default);
    Task ConcluirAsync(Guid id, CancellationToken cancellationToken = default);
    Task CancelarAsync(Guid id, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}
