namespace HexagonalExemplo.Dominio.Eventos;

/// <summary>
/// Evento de domínio disparado quando uma tarefa é concluída
/// Representa um fato de negócio (Domain Event)
/// </summary>
public record TarefaConcluidaEvento
{
    public Guid TarefaId { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public DateTime DataConclusao { get; init; }
    public TimeSpan TempoTotal { get; init; } // Tempo desde criação até conclusão

    public static TarefaConcluidaEvento Criar(Guid tarefaId, string titulo, DateTime dataCriacao, DateTime dataConclusao)
    {
        return new TarefaConcluidaEvento
        {
            TarefaId = tarefaId,
            Titulo = titulo,
            DataConclusao = dataConclusao,
            TempoTotal = dataConclusao - dataCriacao
        };
    }
}
