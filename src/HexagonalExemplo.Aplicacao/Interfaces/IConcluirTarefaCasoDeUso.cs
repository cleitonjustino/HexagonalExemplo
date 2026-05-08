namespace HexagonalExemplo.Aplicacao.Interfaces;

public interface IConcluirTarefaCasoDeUso
{
    Task ConcluirAsync(Guid id, CancellationToken cancellationToken = default);
}
