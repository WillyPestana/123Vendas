using _123Vendas.Core.Business;
using _123Vendas.Core.Dtos;
using _123Vendas.Core.Entities;
using _123Vendas.Core.Repositories._123Vendas;
using _123Vendas.Infrastructure.MessageBus;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace _123Vendas.Application.Business
{
    public class VendaBusiness : IVendaBusiness
    {
        private readonly IRepository<Venda> _vendaRepository;
        private readonly IRepository<ItemVenda> _itemVendaRepository;
        private readonly IMapper _mapper;
        private readonly IMessageBus _messageBus;

        public VendaBusiness(IRepository<Venda> vendaRepository, IRepository<ItemVenda> itemVendaRepository, IMapper mapper, IMessageBus messageBus)
        {
            _vendaRepository = vendaRepository;
            _itemVendaRepository = itemVendaRepository;
            _mapper = mapper;
            _messageBus = messageBus;
        }

        public async Task<VendaDto?> GetByIdAsync(int id)
        {
            var venda = await _vendaRepository.Get(
                v => v.Id == id,
                includes: query => query.Include(v => v.Itens));

            if (venda == null)
                return null;

            return _mapper.Map<VendaDto>(venda);
        }

        public async Task<VendaDto> CriarVendaAsync(VendaDto vendaDto)
        {
            var venda = _mapper.Map<Venda>(vendaDto);

            // Calcular o valor total da venda diretamente com base nos itens válidos
            venda.ValorTotal = venda.Itens
                .Where(i => !i.Cancelado)
                .Sum(i => i.Quantidade * (i.ValorUnitario - i.Desconto));

            await _vendaRepository.Create(venda);
            vendaDto.Id = venda.Id;

            _messageBus.Publish("CompraCriada", $"Venda {venda.Id} foi criada com sucesso.");

            return vendaDto;
        }

        public async Task AtualizarVendaAsync(VendaDto vendaDto)
        {
            var venda = await _vendaRepository.Get(
                v => v.Id == vendaDto.Id,
                includes: query => query.Include(v => v.Itens));

            if (venda == null)
                throw new Exception("Venda não encontrada");

            bool foiCancelada = !venda.Cancelado && vendaDto.Cancelado;

            _mapper.Map(vendaDto, venda);

            // Recalcular o valor total da venda com base nos itens válidos
            venda.ValorTotal = venda.Itens
                .Where(i => !i.Cancelado)
                .Sum(i => i.Quantidade * (i.ValorUnitario - i.Desconto));

            await _vendaRepository.Update(venda);

            if (foiCancelada)
            {
                _messageBus.Publish("CompraCancelada", $"Venda {venda.Id} foi cancelada.");
            }
            else
            {
                _messageBus.Publish("CompraAlterada", $"Venda {venda.Id} foi atualizada.");
            }
        }

        public async Task CancelarVendaAsync(int id)
        {
            var venda = await _vendaRepository.GetById(id);
            if (venda == null)
                throw new Exception("Venda não encontrada");

            venda.Cancelado = true;

            await _vendaRepository.Update(venda);

            _messageBus.Publish("CompraCancelada", $"Venda {venda.Id} foi cancelada.");
        }

        public async Task CancelarItemAsync(int vendaId, int itemId)
        {
            var venda = await _vendaRepository.Get(
                v => v.Id == vendaId,
                includes: query => query.Include(v => v.Itens));

            if (venda == null)
                throw new Exception("Venda não encontrada");

            var item = venda.Itens.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new Exception("Item não encontrado na venda.");

            item.Cancelado = true;

            // Recalcular o valor total da venda após cancelar o item
            venda.ValorTotal = venda.Itens
                .Where(i => !i.Cancelado)
                .Sum(i => i.Quantidade * (i.ValorUnitario - i.Desconto));

            await _vendaRepository.Update(venda);

            _messageBus.Publish("ItemCancelado", $"Item {itemId} da venda {vendaId} foi cancelado.");
        }

        public async Task<List<VendaDto>> ObterTodasVendasAsync()
        {
            var vendas = await _vendaRepository.GetAll(
                includes: query => query.Include(v => v.Itens));

            return _mapper.Map<List<VendaDto>>(vendas);
        }
    }
}
