using FluentValidation;
using HexagonalExemplo.Aplicacao.DTOs;
using HexagonalExemplo.Aplicacao.Interfaces;
using HexagonalExemplo.Aplicacao.Mapeamentos;
using HexagonalExemplo.Dominio.Excecoes;
using HexagonalExemplo.Dominio.Repositorios;

namespace HexagonalExemplo.Aplicacao.CasosDeUso;

public class AtualizarTarefaCasoDeUso : IAtualizarTarefaCasoDeUso
{
    private readonly ITarefaRepositorio _tarefaRepositorio;
    private readonly IValidator<AtualizarTarefaRequest> _atualizarValidador;

    public AtualizarTarefaCasoDeUso(ITarefaRepositorio tarefaRepositorio, IValidator<AtualizarTarefaRequest> atualizarValidador)
    {
        _tarefaRepositorio = tarefaRepositorio ?? throw new ArgumentNullException(nameof(tarefaRepositorio));
        _atualizarValidador = atualizarValidador ?? throw new ArgumentNullException(nameof(atualizarValidador));
    }

    public async Task<TarefaResponse> AtualizarAsync(Guid id, AtualizarTarefaRequest request, CancellationToken cancellationToken = default)
    {
        var validacao = await _atualizarValidador.ValidateAsync(request, cancellationToken);
        if (!validacao.IsValid)
            throw new ValidacaoExcecao(string.Join(", ", validacao.Errors.Select(e => e.ErrorMessage)));

        var tarefa = await _tarefaRepositorio.ObterPorIdAsync(id, cancellationToken)
            ?? throw new TarefaNaoEncontradaExcecao(id);

        tarefa.Atualizar(request.Titulo, request.Descricao);
        await _tarefaRepositorio.AtualizarAsync(tarefa, cancellationToken);

        return tarefa.ParaResponse();
    }
}
