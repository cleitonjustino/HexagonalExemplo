using HexagonalExemplo.Dominio.BarramentoDeMensagens;
using MassTransit;

namespace HexagonalExemplo.Infraestrutura.Mensageria;

/// <summary>
/// Adapter (Secondary/Driven) que implementa o port IBarramentoDeMensagens
/// Usa MassTransit com RabbitMQ/Kafka/Azure Service Bus
/// </summary>
public class BarramentoMassTransit : IBarramentoDeMensagens
{
    private readonly IPublishEndpoint _publishEndpoint;

    public BarramentoMassTransit(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task PublicarAsync<T>(T evento, CancellationToken cancellationToken = default) where T : class
    {
        await _publishEndpoint.Publish(evento, cancellationToken);
    }

    public async Task PublicarBatchAsync<T>(IEnumerable<T> eventos, CancellationToken cancellationToken = default) where T : class
    {
        var tasks = eventos.Select(e => _publishEndpoint.Publish(e, cancellationToken));
        await Task.WhenAll(tasks);
    }
}
