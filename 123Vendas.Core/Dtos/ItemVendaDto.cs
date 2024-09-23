namespace _123Vendas.Core.Dtos
{
    public class ItemVendaDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string DescricaoProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Desconto { get; set; }
        public bool Cancelado { get; set; }
    }
}
