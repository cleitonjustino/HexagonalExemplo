namespace HexagonalExemplo.Aplicacao.Interfaces;

public interface ICancelarTarefaCasoDeUso
{
    Task CancelarAsync(Guid id, CancellationToken cancellationToken = default);
}
