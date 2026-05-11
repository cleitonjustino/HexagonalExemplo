using FluentValidation;
using HexagonalExemplo.Aplicacao.DTOs;
using HexagonalExemplo.Aplicacao.Interfaces;
using HexagonalExemplo.Aplicacao.Mapeamentos;
using HexagonalExemplo.Dominio.Entidades;
using HexagonalExemplo.Dominio.Excecoes;
using HexagonalExemplo.Dominio.Repositorios;

namespace HexagonalExemplo.Aplicacao.CasosDeUso.Tarefas;

public class CriarTarefaCasoDeUso : ICriarTarefaCasoDeUso
{
    private readonly ITarefaRepositorio _tarefaRepositorio;
    private readonly IValidator<CriarTarefaRequest> _criarValidador;

    public CriarTarefaCasoDeUso(ITarefaRepositorio tarefaRepositorio, IValidator<CriarTarefaRequest> criarValidador)
    {
        _tarefaRepositorio = tarefaRepositorio ?? throw new ArgumentNullException(nameof(tarefaRepositorio));
        _criarValidador = criarValidador ?? throw new ArgumentNullException(nameof(criarValidador));
    }

    public async Task<TarefaResponse> CriarAsync(CriarTarefaRequest request, CancellationToken cancellationToken = default)
    {
        var validacao = await _criarValidador.ValidateAsync(request, cancellationToken);
        if (!validacao.IsValid)
            throw new ValidacaoExcecao(string.Join(", ", validacao.Errors.Select(e => e.ErrorMessage)));

        var tarefa = new Tarefa(request.Titulo, request.Descricao);
        await _tarefaRepositorio.AdicionarAsync(tarefa, cancellationToken);
        return tarefa.ParaResponse();
    }
}
