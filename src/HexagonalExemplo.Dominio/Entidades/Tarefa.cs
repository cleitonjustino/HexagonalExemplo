using HexagonalExemplo.Dominio.ObjetosDeValor;

namespace HexagonalExemplo.Dominio.Entidades;

public class Tarefa
{
    public Guid Id { get; private set; }
    public string Titulo { get; private set; }
    public string Descricao { get; private set; }
    public StatusTarefa Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataConclusao { get; private set; }

    public Tarefa(string titulo, string? descricao)
    {
        Id = Guid.NewGuid();
        Titulo = titulo ?? throw new ArgumentNullException(nameof(titulo));
        Descricao = descricao ?? string.Empty;
        Status = StatusTarefa.Pendente;
        DataCriacao = DateTime.UtcNow;
    }

    public void Concluir()
    {
        if (Status == StatusTarefa.Concluida)
            throw new InvalidOperationException("Tarefa já está concluída");

        Status = StatusTarefa.Concluida;
        DataConclusao = DateTime.UtcNow;
    }

    public void Cancelar()
    {
        if (Status == StatusTarefa.Concluida)
            throw new InvalidOperationException("Não é possível cancelar uma tarefa concluída");

        Status = StatusTarefa.Cancelada;
    }

    public void Atualizar(string titulo, string descricao)
    {
        if (Status == StatusTarefa.Concluida)
            throw new InvalidOperationException("Não é possível editar uma tarefa concluída");

        Titulo = titulo ?? throw new ArgumentNullException(nameof(titulo));
        Descricao = descricao ?? string.Empty;
    }
}
