namespace HexagonalExemplo.Aplicacao.DTOs;

public record CriarTarefaRequest(
    string Titulo,
    string Descricao
);

public record AtualizarTarefaRequest(
    string Titulo,
    string Descricao
);

public record TarefaResponse(
    Guid Id,
    string Titulo,
    string Descricao,
    string Status,
    DateTime DataCriacao,
    DateTime? DataConclusao
);
