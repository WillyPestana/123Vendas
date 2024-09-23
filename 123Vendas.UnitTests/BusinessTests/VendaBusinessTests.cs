using FluentAssertions;
using NSubstitute;
using _123Vendas.Application.Business;
using _123Vendas.Core.Business;
using _123Vendas.Core.Repositories._123Vendas;
using _123Vendas.Core.Entities;
using _123Vendas.Core.Dtos;
using _123Vendas.Infrastructure.MessageBus;
using Bogus;
using Xunit;
using System.Linq.Expressions;

namespace _123Vendas.UnitTests.BusinessTests
{
    public class VendaBusinessTests
    {
        private readonly IVendaBusiness _vendaBusiness;
        private readonly IRepository<Venda> _vendaRepository = Substitute.For<IRepository<Venda>>();
        private readonly IRepository<ItemVenda> _itemVendaRepository = Substitute.For<IRepository<ItemVenda>>();
        private readonly IMessageBus _messageBus = Substitute.For<IMessageBus>();
        private readonly Faker<VendaDto> _vendaDtoFaker;
        private readonly Faker<Venda> _vendaFaker;
        private readonly Faker<ItemVendaDto> _itemVendaDtoFaker;
        private readonly Faker<ItemVenda> _itemVendaFaker;

        public VendaBusinessTests()
        {
            var mapper = AutoMapperHelper.GetMapper();
            _vendaBusiness = new VendaBusiness(_vendaRepository, _itemVendaRepository, mapper, _messageBus);

            // Setup Faker for VendaDto
            _itemVendaDtoFaker = new Faker<ItemVendaDto>()
                .RuleFor(i => i.ProdutoId, f => f.Random.Int(1, 100))
                .RuleFor(i => i.DescricaoProduto, f => f.Commerce.ProductName())
                .RuleFor(i => i.Quantidade, f => f.Random.Int(1, 10))
                .RuleFor(i => i.ValorUnitario, f => f.Finance.Amount(10, 500));

            _vendaDtoFaker = new Faker<VendaDto>()
                .RuleFor(v => v.NumeroVenda, f => f.Random.String2(5, "0123456789"))
                .RuleFor(v => v.ClienteId, f => f.Random.Int(1, 100))
                .RuleFor(v => v.NomeCliente, f => f.Person.FullName)
                .RuleFor(v => v.Itens, f => _itemVendaDtoFaker.Generate(3))
                .RuleFor(v => v.ValorTotal, f => f.Finance.Amount(100, 1000));

            // Setup Faker for Venda entity
            _itemVendaFaker = new Faker<ItemVenda>()
                .RuleFor(i => i.Id, f => f.Random.Int(1, 100))
                .RuleFor(i => i.ProdutoId, f => f.Random.Int(1, 100))
                .RuleFor(i => i.DescricaoProduto, f => f.Commerce.ProductName())
                .RuleFor(i => i.Quantidade, f => f.Random.Int(1, 10))
                .RuleFor(i => i.ValorUnitario, f => f.Finance.Amount(10, 500));

            _vendaFaker = new Faker<Venda>()
                .RuleFor(v => v.Id, f => f.Random.Int(1, 100))
                .RuleFor(v => v.NumeroVenda, f => f.Random.String2(5, "0123456789"))
                .RuleFor(v => v.ClienteId, f => f.Random.Int(1, 100))
                .RuleFor(v => v.NomeCliente, f => f.Person.FullName)
                .RuleFor(v => v.Itens, f => _itemVendaFaker.Generate(3))
                .RuleFor(v => v.ValorTotal, f => f.Finance.Amount(100, 1000))
                .RuleFor(v => v.Cancelado, f => f.Random.Bool());
        }

        private Venda CreateFakeVendaWithItens()
        {
            var venda = _vendaFaker.Generate();
            venda.Itens = _itemVendaFaker.Generate(3);
            return venda;
        }

        [Fact]
        public async Task CriarVenda_DevePublicarEvento_VendaCriada()
        {
            // Arrange
            var vendaDto = _vendaDtoFaker.Generate();

            // Act
            var result = await _vendaBusiness.CriarVendaAsync(vendaDto);

            // Assert
            result.Should().NotBeNull();
            result.NumeroVenda.Should().Be(vendaDto.NumeroVenda);
            _messageBus.Received(1).Publish("CompraCriada", Arg.Any<string>());
        }

        [Fact]
        public async Task AtualizarVenda_SeVendaCancelada_DevePublicarEvento_Cancelamento()
        {
            // Arrange
            var venda = CreateFakeVendaWithItens();
            venda.Cancelado = false;
            Expression<Func<Venda, bool>> filter = v => v.Id == venda.Id;
            _vendaRepository.Get(filter, Arg.Any<Func<IQueryable<Venda>, IQueryable<Venda>>>()).Returns(venda);

            var vendaDto = _vendaDtoFaker.Generate();
            vendaDto.Id = venda.Id;
            vendaDto.Cancelado = true;

            // Act
            await _vendaBusiness.AtualizarVendaAsync(vendaDto);

            // Assert
            _messageBus.Received(1).Publish("CompraCancelada", Arg.Any<string>());
        }

        [Fact]
        public async Task AtualizarVenda_SeNaoCancelada_DevePublicarEvento_Alteracao()
        {
            // Arrange
            var venda = CreateFakeVendaWithItens();
            venda.Cancelado = false;
            Expression<Func<Venda, bool>> filter = v => v.Id == venda.Id;
            _vendaRepository.Get(filter, Arg.Any<Func<IQueryable<Venda>, IQueryable<Venda>>>()).Returns(venda);

            var vendaDto = _vendaDtoFaker.Generate();
            vendaDto.Id = venda.Id;
            vendaDto.Cancelado = false;

            // Act
            await _vendaBusiness.AtualizarVendaAsync(vendaDto);

            // Assert
            _messageBus.Received(1).Publish("CompraAlterada", Arg.Any<string>());
        }

        [Fact]
        public async Task CancelarVenda_DevePublicarEvento_Cancelamento()
        {
            // Arrange
            var venda = CreateFakeVendaWithItens();
            venda.Cancelado = false;
            Expression<Func<Venda, bool>> filter = v => v.Id == venda.Id;
            _vendaRepository.Get(filter, Arg.Any<Func<IQueryable<Venda>, IQueryable<Venda>>>()).Returns(venda);

            // Act
            await _vendaBusiness.CancelarVendaAsync(venda.Id);

            // Assert
            _messageBus.Received(1).Publish("CompraCancelada", Arg.Any<string>());
        }

        [Fact]
        public async Task CancelarItem_DevePublicarEvento_ItemCancelado()
        {
            // Arrange
            var venda = CreateFakeVendaWithItens();
            Expression<Func<Venda, bool>> filter = v => v.Id == venda.Id;
            _vendaRepository.Get(filter, Arg.Any<Func<IQueryable<Venda>, IQueryable<Venda>>>()).Returns(venda);

            var itemVenda = venda.Itens.First();

            // Act
            await _vendaBusiness.CancelarItemAsync(venda.Id, itemVenda.Id);

            // Assert
            _messageBus.Received(1).Publish("ItemCancelado", Arg.Any<string>());
        }

        [Fact]
        public async Task GetById_DeveRetornarVenda()
        {
            // Arrange
            var venda = CreateFakeVendaWithItens();
            Expression<Func<Venda, bool>> filter = v => v.Id == venda.Id;
            _vendaRepository.Get(filter, Arg.Any<Func<IQueryable<Venda>, IQueryable<Venda>>>()).Returns(venda);

            // Act
            var result = await _vendaBusiness.GetByIdAsync(venda.Id);

            // Assert
            result.Should().NotBeNull();
            result.NumeroVenda.Should().Be(venda.NumeroVenda);
        }

        [Fact]
        public async Task ObterTodasVendas_DeveRetornarListaDeVendas()
        {
            // Arrange
            var vendas = _vendaFaker.Generate(2);
            _vendaRepository.GetAll().Returns(vendas);

            // Act
            var result = await _vendaBusiness.ObterTodasVendasAsync();

            // Assert
            result.Should().NotBeEmpty();
            result.Count.Should().Be(2);
        }
    }
}
