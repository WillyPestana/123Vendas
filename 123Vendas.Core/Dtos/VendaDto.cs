namespace _123Vendas.Core.Dtos
{
    public class VendaDto
    {
        public int Id { get; set; }
        public string NumeroVenda { get; set; }
        public DateTime DataVenda { get; set; }

        public int ClienteId { get; set; }
        public string NomeCliente { get; set; }

        public int FilialId { get; set; }
        public string NomeFilial { get; set; }

        public List<ItemVendaDto> Itens { get; set; }
        public decimal ValorTotal { get; set; }
        public bool Cancelado { get; set; }
    }
}
