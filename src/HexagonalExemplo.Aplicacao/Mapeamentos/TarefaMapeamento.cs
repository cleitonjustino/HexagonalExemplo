using HexagonalExemplo.Aplicacao.DTOs;
using HexagonalExemplo.Dominio.Entidades;

namespace HexagonalExemplo.Aplicacao.Mapeamentos;

public static class TarefaMapeamento
{
    public static TarefaResponse ParaResponse(this Tarefa tarefa)
    {
        return new TarefaResponse(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Descricao,
            tarefa.Status.ToString(),
            tarefa.DataCriacao,
            tarefa.DataConclusao
        );
    }

    public static IEnumerable<TarefaResponse> ParaResponseList(this IEnumerable<Tarefa> tarefas)
    {
        return tarefas.Select(ParaResponse);
    }
}
