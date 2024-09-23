namespace _123Vendas.Core.Entities
{
    public partial class ItemVenda
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string DescricaoProduto { get; set; }

        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Desconto { get; set; }

        public decimal ValorTotal { get; private set; }

        public int VendaId { get; set; }
        public Venda Venda { get; set; }

        public bool Cancelado { get; set; }
    }
}
