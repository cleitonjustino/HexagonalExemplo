namespace HexagonalExemplo.Aplicacao.Interfaces;

public interface IRemoverTarefaCasoDeUso
{
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}
