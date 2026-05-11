using FluentMigrator;

namespace HexagonalExemplo.Infraestrutura.Migrations;

[Migration(1)]
public class _001_CreateTarefas : Migration
{
    public override void Up()
    {
        Create.Table("Tarefas")
            .WithColumn("Id").AsString().PrimaryKey().NotNullable()
            .WithColumn("Titulo").AsString().NotNullable()
            .WithColumn("Descricao").AsString().Nullable()
            .WithColumn("Status").AsInt32().NotNullable()
            .WithColumn("DataCriacao").AsString().NotNullable()
            .WithColumn("DataConclusao").AsString().Nullable();

        Create.Index("IX_Tarefas_Status").OnTable("Tarefas").OnColumn("Status");
    }

    public override void Down()
    {
        Delete.Index("IX_Tarefas_Status").OnTable("Tarefas");
        Delete.Table("Tarefas");
    }
}
