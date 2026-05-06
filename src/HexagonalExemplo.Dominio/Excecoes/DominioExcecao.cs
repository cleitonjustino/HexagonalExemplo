namespace HexagonalExemplo.Dominio.Excecoes;

public class DominioExcecao : Exception
{
    public DominioExcecao(string mensagem) : base(mensagem) { }
}

public class TarefaNaoEncontradaExcecao : DominioExcecao
{
    public TarefaNaoEncontradaExcecao(Guid id) : base($"Tarefa com ID {id} não encontrada") { }
}

public class ValidacaoExcecao : DominioExcecao
{
    public ValidacaoExcecao(string mensagem) : base(mensagem) { }
}
