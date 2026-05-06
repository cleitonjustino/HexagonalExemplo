using FluentValidation;
using HexagonalExemplo.Aplicacao.DTOs;

namespace HexagonalExemplo.Aplicacao.Validadores;

public class AtualizarTarefaRequestValidador : AbstractValidator<AtualizarTarefaRequest>
{
    public AtualizarTarefaRequestValidador()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("O título é obrigatório")
            .MinimumLength(3)
            .WithMessage("O título deve ter no mínimo 3 caracteres")
            .MaximumLength(200)
            .WithMessage("O título deve ter no máximo 200 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000)
            .WithMessage("A descrição deve ter no máximo 1000 caracteres");
    }
}
