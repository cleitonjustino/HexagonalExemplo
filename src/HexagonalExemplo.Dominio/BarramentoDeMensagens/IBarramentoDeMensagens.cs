using HexagonalExemplo.Dominio.Eventos;

namespace HexagonalExemplo.Dominio.BarramentoDeMensagens;

/// <summary>
/// Port (Secondary/Driven) para publicação de eventos de domínio
/// Abstração do message broker (RabbitMQ, Kafka, Azure Service Bus, etc)
/// </summary>
public interface IBarramentoDeMensagens
{
    Task PublicarAsync<T>(T evento, CancellationToken cancellationToken = default) where T : class;
    
    Task PublicarBatchAsync<T>(IEnumerable<T> eventos, CancellationToken cancellationToken = default) where T : class;
}
