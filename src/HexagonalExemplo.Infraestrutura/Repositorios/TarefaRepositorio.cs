using Dapper;
using HexagonalExemplo.Dominio.Entidades;
using HexagonalExemplo.Dominio.ObjetosDeValor;
using HexagonalExemplo.Dominio.Repositorios;
using HexagonalExemplo.Infraestrutura.Persistencia;
using System.Linq;

namespace HexagonalExemplo.Infraestrutura.Repositorios;

public class TarefaRepositorio : ITarefaRepositorio
{
    private readonly IConexaoDapper _conexaoDapper;

    public TarefaRepositorio(IConexaoDapper conexaoDapper)
    {
        _conexaoDapper = conexaoDapper ?? throw new ArgumentNullException(nameof(conexaoDapper));
    }

    public async Task<Tarefa?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var conexao = _conexaoDapper.CriarConexao();
        var sql = "SELECT * FROM Tarefas WHERE Id = @Id";
        var resultado = await conexao.QueryFirstOrDefaultAsync(sql, new { Id = id.ToString() });
        return resultado == null ? null : MapearParaTarefa(resultado);
    }

    public async Task<IEnumerable<Tarefa>> ListarTodasAsync(CancellationToken cancellationToken = default)
    {
        using var conexao = _conexaoDapper.CriarConexao();
        var sql = "SELECT * FROM Tarefas ORDER BY DataCriacao DESC";
        var resultados = await conexao.QueryAsync(sql);
        return resultados.Select(MapearParaTarefa);
    }

    public async Task<IEnumerable<Tarefa>> ListarPorStatusAsync(StatusTarefa status, CancellationToken cancellationToken = default)
    {
        using var conexao = _conexaoDapper.CriarConexao();
        var sql = "SELECT * FROM Tarefas WHERE Status = @Status ORDER BY DataCriacao DESC";
        var resultados = await conexao.QueryAsync(sql, new { Status = (int)status });
        return resultados.Select(MapearParaTarefa);
    }

    private static Tarefa MapearParaTarefa(dynamic row)
    {
        var tarefa = new Tarefa(row.Titulo, row.Descricao);
        // Usar reflection para definir o Id e outras propriedades
        typeof(Tarefa).GetProperty("Id")?.SetValue(tarefa, Guid.Parse(row.Id));
        typeof(Tarefa).GetProperty("Status")?.SetValue(tarefa, (StatusTarefa)row.Status);
        typeof(Tarefa).GetProperty("DataCriacao")?.SetValue(tarefa, DateTime.Parse(row.DataCriacao));
        if (row.DataConclusao != null)
            typeof(Tarefa).GetProperty("DataConclusao")?.SetValue(tarefa, DateTime.Parse(row.DataConclusao));
        return tarefa;
    }

    public async Task AdicionarAsync(Tarefa tarefa, CancellationToken cancellationToken = default)
    {
        using var conexao = _conexaoDapper.CriarConexao();
        var sql = @"
            INSERT INTO Tarefas (Id, Titulo, Descricao, Status, DataCriacao, DataConclusao)
            VALUES (@Id, @Titulo, @Descricao, @Status, @DataCriacao, @DataConclusao)";

        await conexao.ExecuteAsync(sql, new
        {
            Id = tarefa.Id.ToString(),
            tarefa.Titulo,
            tarefa.Descricao,
            Status = (int)tarefa.Status,
            DataCriacao = tarefa.DataCriacao.ToString("O"),
            DataConclusao = tarefa.DataConclusao?.ToString("O")
        });
    }

    public async Task AtualizarAsync(Tarefa tarefa, CancellationToken cancellationToken = default)
    {
        using var conexao = _conexaoDapper.CriarConexao();
        var sql = @"
            UPDATE Tarefas 
            SET Titulo = @Titulo, 
                Descricao = @Descricao, 
                Status = @Status, 
                DataConclusao = @DataConclusao
            WHERE Id = @Id";

        await conexao.ExecuteAsync(sql, new
        {
            Id = tarefa.Id.ToString(),
            tarefa.Titulo,
            tarefa.Descricao,
            Status = (int)tarefa.Status,
            DataConclusao = tarefa.DataConclusao?.ToString("O")
        });
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var conexao = _conexaoDapper.CriarConexao();
        var sql = "DELETE FROM Tarefas WHERE Id = @Id";
        await conexao.ExecuteAsync(sql, new { Id = id.ToString() });
    }
}
