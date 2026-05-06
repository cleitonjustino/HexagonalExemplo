using HexagonalExemplo.Dominio.Entidades;

namespace HexagonalExemplo.Dominio.Repositorios;

public interface ITarefaRepositorio
{
    Task<Tarefa?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tarefa>> ListarTodasAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Tarefa>> ListarPorStatusAsync(ObjetosDeValor.StatusTarefa status, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Tarefa tarefa, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Tarefa tarefa, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}
