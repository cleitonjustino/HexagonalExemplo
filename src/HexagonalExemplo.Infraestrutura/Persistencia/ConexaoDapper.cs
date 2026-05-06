using System.Data;
using Microsoft.Data.Sqlite;

namespace HexagonalExemplo.Infraestrutura.Persistencia;

public interface IConexaoDapper : IDisposable
{
    IDbConnection CriarConexao();
}

public class ConexaoDapper : IConexaoDapper
{
    private readonly SqliteConnection _conexaoMantida;

    public ConexaoDapper(string connectionString)
    {
        // Manter conexão aberta para SQLite em memória funcionar
        _conexaoMantida = new SqliteConnection(connectionString);
        _conexaoMantida.Open();
    }

    public IDbConnection CriarConexao()
    {
        // Retorna nova conexão compartilhando o cache em memória
        var conexao = new SqliteConnection(_conexaoMantida.ConnectionString);
        conexao.Open();
        return conexao;
    }

    public void Dispose()
    {
        _conexaoMantida?.Close();
        _conexaoMantida?.Dispose();
    }
}
