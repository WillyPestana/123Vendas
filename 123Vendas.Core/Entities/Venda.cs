namespace _123Vendas.Core.Entities
{
    public partial class Venda
    {
        public int Id { get; set; }
        public string NumeroVenda { get; set; }
        public DateTime DataVenda { get; set; }

        public int ClienteId { get; set; }
        public string NomeCliente { get; set; }

        public int FilialId { get; set; }
        public string NomeFilial { get; set; }

        public virtual ICollection<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
        public decimal ValorTotal { get; set; }
        public bool Cancelado { get; set; }
    }
}
