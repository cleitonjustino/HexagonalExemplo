using FluentAssertions;
using FluentValidation.TestHelper;
using HexagonalExemplo.Aplicacao.DTOs;
using HexagonalExemplo.Aplicacao.Validadores;
using Xunit;

namespace HexagonalExemplo.Testes.Aplicacao.Validadores;

public class CriarTarefaRequestValidadorTestes
{
    private readonly CriarTarefaRequestValidador _validador;

    public CriarTarefaRequestValidadorTestes()
    {
        _validador = new CriarTarefaRequestValidador();
    }

    [Fact]
    public void Validar_TituloVazio_DeveTerErro()
    {
        // Arrange
        var request = new CriarTarefaRequest("", "Descrição");

        // Act
        var resultado = _validador.TestValidate(request);

        // Assert
        resultado.ShouldHaveValidationErrorFor(x => x.Titulo);
    }

    [Fact]
    public void Validar_TituloNulo_DeveTerErro()
    {
        // Arrange
        var request = new CriarTarefaRequest(null!, "Descrição");

        // Act
        var resultado = _validador.TestValidate(request);

        // Assert
        resultado.ShouldHaveValidationErrorFor(x => x.Titulo);
    }

    [Fact]
    public void Validar_TituloMuitoCurto_DeveTerErro()
    {
        // Arrange
        var request = new CriarTarefaRequest("AB", "Descrição");

        // Act
        var resultado = _validador.TestValidate(request);

        // Assert
        resultado.ShouldHaveValidationErrorFor(x => x.Titulo);
    }

    [Fact]
    public void Validar_TituloMuitoLongo_DeveTerErro()
    {
        // Arrange
        var request = new CriarTarefaRequest(new string('A', 201), "Descrição");

        // Act
        var resultado = _validador.TestValidate(request);

        // Assert
        resultado.ShouldHaveValidationErrorFor(x => x.Titulo);
    }

    [Fact]
    public void Validar_DescricaoMuitoLonga_DeveTerErro()
    {
        // Arrange
        var request = new CriarTarefaRequest("Título Válido", new string('A', 1001));

        // Act
        var resultado = _validador.TestValidate(request);

        // Assert
        resultado.ShouldHaveValidationErrorFor(x => x.Descricao);
    }

    [Fact]
    public void Validar_DadosValidos_NaoDeveTerErros()
    {
        // Arrange
        var request = new CriarTarefaRequest("Título Válido", "Descrição válida");

        // Act
        var resultado = _validador.TestValidate(request);

        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_SemDescricao_NaoDeveTerErros()
    {
        // Arrange
        var request = new CriarTarefaRequest("Título Válido", null!);

        // Act
        var resultado = _validador.TestValidate(request);

        // Assert
        resultado.ShouldNotHaveValidationErrorFor(x => x.Descricao);
    }
}
