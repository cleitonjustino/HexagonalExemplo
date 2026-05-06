using HexagonalExemplo.Aplicacao.DTOs;
using HexagonalExemplo.Aplicacao.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HexagonalExemplo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TarefasController : ControllerBase
{
    private readonly ITarefaCasoDeUso _tarefaCasoDeUso;

    public TarefasController(ITarefaCasoDeUso tarefaCasoDeUso)
    {
        _tarefaCasoDeUso = tarefaCasoDeUso ?? throw new ArgumentNullException(nameof(tarefaCasoDeUso));
    }

    /// <summary>
    /// Lista todas as tarefas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TarefaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TarefaResponse>>> ListarTodas(
        CancellationToken cancellationToken = default)
    {
        var tarefas = await _tarefaCasoDeUso.ListarTodasAsync(cancellationToken);
        return Ok(tarefas);
    }

    /// <summary>
    /// Obtém uma tarefa pelo ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TarefaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TarefaResponse>> ObterPorId(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaCasoDeUso.ObterPorIdAsync(id, cancellationToken);
        if (tarefa == null)
            return NotFound(new { mensagem = $"Tarefa com ID {id} não encontrada" });

        return Ok(tarefa);
    }

    /// <summary>
    /// Cria uma nova tarefa
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TarefaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TarefaResponse>> Criar(
        [FromBody] CriarTarefaRequest request,
        CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaCasoDeUso.CriarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
    }

    /// <summary>
    /// Atualiza uma tarefa existente
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TarefaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TarefaResponse>> Atualizar(
        [FromRoute] Guid id,
        [FromBody] AtualizarTarefaRequest request,
        CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaCasoDeUso.AtualizarAsync(id, request, cancellationToken);
        return Ok(tarefa);
    }

    /// <summary>
    /// Marca uma tarefa como concluída
    /// </summary>
    [HttpPatch("{id:guid}/concluir")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Concluir(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        await _tarefaCasoDeUso.ConcluirAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Cancela uma tarefa
    /// </summary>
    [HttpPatch("{id:guid}/cancelar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancelar(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        await _tarefaCasoDeUso.CancelarAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Remove uma tarefa
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        await _tarefaCasoDeUso.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}
