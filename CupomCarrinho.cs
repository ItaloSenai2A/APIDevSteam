public class CupomCarrinho
{
    public Guid CupomCarrinhoId { get; set; }
    public string Nome { get; set; } // Alterado de Guid para string
    public int Desconto { get; set; }
    public DateTime? DataValidade { get; set; }
    public bool? Ativo { get; set; }
    public DateTime? DataCriacao { get; set; }
    public int? LimiteUso { get; set; }
}
