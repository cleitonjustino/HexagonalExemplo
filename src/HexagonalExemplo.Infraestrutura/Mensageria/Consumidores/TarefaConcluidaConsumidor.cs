using HexagonalExemplo.Dominio.Eventos;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace HexagonalExemplo.Infraestrutura.Mensageria.Consumidores;

/// <summary>
/// Consumer (Adapter de entrada) que processa eventos de tarefa concluída
/// Simula um serviço externo de notificação (email, push, relatórios)
/// </summary>
public class TarefaConcluidaConsumidor : IConsumer<TarefaConcluidaEvento>
{
    private readonly ILogger<TarefaConcluidaConsumidor> _logger;

    public TarefaConcluidaConsumidor(ILogger<TarefaConcluidaConsumidor> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<TarefaConcluidaEvento> context)
    {
        var evento = context.Message;

        _logger.LogInformation(
            "Tarefa concluída recebida: {TarefaId} - {Titulo}. Tempo total: {TempoTotal}",
            evento.TarefaId,
            evento.Titulo,
            evento.TempoTotal);

        // Simula envio de email de notificação
        await EnviarEmailDeConclusaoAsync(evento);

        // Simula atualização de relatório/dashboard
        await AtualizarMetricasAsync(evento);

        _logger.LogInformation("Processamento do evento {TarefaId} concluído", evento.TarefaId);
    }

    private async Task EnviarEmailDeConclusaoAsync(TarefaConcluidaEvento evento)
    {
        _logger.LogInformation("Enviando email de conclusão para tarefa: {Titulo}", evento.Titulo);
        await Task.Delay(100);
    }

    private async Task AtualizarMetricasAsync(TarefaConcluidaEvento evento)
    {
        _logger.LogInformation("Atualizando métricas no dashboard");
        await Task.Delay(50); 
    }
}
