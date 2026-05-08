using HexagonalExemplo.Aplicacao.DTOs;
using HexagonalExemplo.Aplicacao.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HexagonalExemplo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TarefasController : ControllerBase
{
    private readonly IListarTarefasCasoDeUso _listarCasoDeUso;
    private readonly IObterTarefaPorIdCasoDeUso _obterPorIdCasoDeUso;
    private readonly ICriarTarefaCasoDeUso _criarCasoDeUso;
    private readonly IAtualizarTarefaCasoDeUso _atualizarCasoDeUso;
    private readonly IConcluirTarefaCasoDeUso _concluirCasoDeUso;
    private readonly ICancelarTarefaCasoDeUso _cancelarCasoDeUso;
    private readonly IRemoverTarefaCasoDeUso _removerCasoDeUso;

    public TarefasController(
        IListarTarefasCasoDeUso listarCasoDeUso,
        IObterTarefaPorIdCasoDeUso obterPorIdCasoDeUso,
        ICriarTarefaCasoDeUso criarCasoDeUso,
        IAtualizarTarefaCasoDeUso atualizarCasoDeUso,
        IConcluirTarefaCasoDeUso concluirCasoDeUso,
        ICancelarTarefaCasoDeUso cancelarCasoDeUso,
        IRemoverTarefaCasoDeUso removerCasoDeUso)
    {
        _listarCasoDeUso = listarCasoDeUso ?? throw new ArgumentNullException(nameof(listarCasoDeUso));
        _obterPorIdCasoDeUso = obterPorIdCasoDeUso ?? throw new ArgumentNullException(nameof(obterPorIdCasoDeUso));
        _criarCasoDeUso = criarCasoDeUso ?? throw new ArgumentNullException(nameof(criarCasoDeUso));
        _atualizarCasoDeUso = atualizarCasoDeUso ?? throw new ArgumentNullException(nameof(atualizarCasoDeUso));
        _concluirCasoDeUso = concluirCasoDeUso ?? throw new ArgumentNullException(nameof(concluirCasoDeUso));
        _cancelarCasoDeUso = cancelarCasoDeUso ?? throw new ArgumentNullException(nameof(cancelarCasoDeUso));
        _removerCasoDeUso = removerCasoDeUso ?? throw new ArgumentNullException(nameof(removerCasoDeUso));
    }

    /// <summary>
    /// Lista todas as tarefas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TarefaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TarefaResponse>>> ListarTodas(
        CancellationToken cancellationToken = default)
    {
        var tarefas = await _listarCasoDeUso.ListarTodasAsync(cancellationToken);
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
        var tarefa = await _obterPorIdCasoDeUso.ObterPorIdAsync(id, cancellationToken);
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
        var tarefa = await _criarCasoDeUso.CriarAsync(request, cancellationToken);
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
        var tarefa = await _atualizarCasoDeUso.AtualizarAsync(id, request, cancellationToken);
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
        await _concluirCasoDeUso.ConcluirAsync(id, cancellationToken);
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
        await _cancelarCasoDeUso.CancelarAsync(id, cancellationToken);
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
        await _removerCasoDeUso.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}
