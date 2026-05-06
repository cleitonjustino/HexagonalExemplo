using Dapper;

namespace HexagonalExemplo.Infraestrutura.Persistencia;

public static class InicializadorBancoDados
{
    public static void Inicializar(IConexaoDapper conexaoDapper)
    {
        using var conexao = conexaoDapper.CriarConexao();

        var sql = @"
            CREATE TABLE IF NOT EXISTS Tarefas (
                Id TEXT PRIMARY KEY,
                Titulo TEXT NOT NULL,
                Descricao TEXT,
                Status INTEGER NOT NULL,
                DataCriacao TEXT NOT NULL,
                DataConclusao TEXT
            );

            CREATE INDEX IF NOT EXISTS IX_Tarefas_Status ON Tarefas(Status);
        ";

        conexao.Execute(sql);
    }
}
