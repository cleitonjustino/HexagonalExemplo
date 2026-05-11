using HexagonalExemplo.Aplicacao.DTOs;
using HexagonalExemplo.Dominio.Entidades;

namespace HexagonalExemplo.Aplicacao.Mapeamentos;

//TODO : Usar AutoMapper para evitar código repetitivo e facilitar a manutenção dos mapeamentos entre entidades e DTOs.
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
