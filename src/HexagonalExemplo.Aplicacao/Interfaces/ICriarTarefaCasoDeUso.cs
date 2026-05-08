using HexagonalExemplo.Aplicacao.DTOs;

namespace HexagonalExemplo.Aplicacao.Interfaces;

public interface ICriarTarefaCasoDeUso
{
    Task<TarefaResponse> CriarAsync(CriarTarefaRequest request, CancellationToken cancellationToken = default);
}
