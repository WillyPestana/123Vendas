using Testcontainers.MsSql;
using Microsoft.EntityFrameworkCore;
using _123Vendas.Infrastructure.Persistence.Context;
using _123Vendas.Core.Entities;
using FluentAssertions;

namespace _123Vendas.IntegrationTests.DatabaseTests
{
    public class VendaIntegrationTests : IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer;
        private _123VendasDbContext _dbContext;

        public VendaIntegrationTests()
        {
            _dbContainer = new MsSqlBuilder()
                .WithPassword("Password123!")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

            var options = new DbContextOptionsBuilder<_123VendasDbContext>()
                .UseSqlServer(_dbContainer.GetConnectionString())
                .Options;

            _dbContext = new _123VendasDbContext(options);
            await _dbContext.Database.EnsureCreatedAsync();
        }

        public async Task DisposeAsync()
        {
            await _dbContainer.StopAsync();
        }

        [Fact]
        public async Task CriarVenda_DeveSalvarNoBancoDeDados()
        {
            // Arrange
            var venda = new Venda
            {
                NumeroVenda = "12345",
                ClienteId = 1,
                NomeCliente = "Cliente Teste",
                NomeFilial = "Filial Teste",
                DataVenda = DateTime.UtcNow,
                Itens = new List<ItemVenda>
                {
                    new ItemVenda { ProdutoId = 1, DescricaoProduto = "Produto 1", Quantidade = 2, ValorUnitario = 100 }
                },
                ValorTotal = 200,
                Cancelado = false
            };

            // Act
            _dbContext.Vendas.Add(venda);
            await _dbContext.SaveChangesAsync();

            // Assert
            var vendaSalva = await _dbContext.Vendas.Include(v => v.Itens).FirstOrDefaultAsync(v => v.NumeroVenda == "12345");
            vendaSalva.Should().NotBeNull();
            vendaSalva.NumeroVenda.Should().Be("12345");
            vendaSalva.Itens.Should().HaveCount(1);
        }

        [Fact]
        public async Task AtualizarVenda_DeveAlterarDadosNoBancoDeDados()
        {
            // Arrange
            var venda = new Venda
            {
                NumeroVenda = "12345",
                ClienteId = 1,
                NomeCliente = "Cliente Teste",
                NomeFilial = "Filial Teste",
                DataVenda = DateTime.UtcNow,
                Itens = new List<ItemVenda>
                {
                    new ItemVenda { ProdutoId = 1, DescricaoProduto = "Produto 1", Quantidade = 2, ValorUnitario = 100 }
                },
                ValorTotal = 200,
                Cancelado = false
            };

            _dbContext.Vendas.Add(venda);
            await _dbContext.SaveChangesAsync();

            // Act
            venda.NomeCliente = "Cliente Atualizado";
            _dbContext.Vendas.Update(venda);
            await _dbContext.SaveChangesAsync();

            // Assert
            var vendaAtualizada = await _dbContext.Vendas.FirstOrDefaultAsync(v => v.NumeroVenda == "12345");
            vendaAtualizada.NomeCliente.Should().Be("Cliente Atualizado");
        }

        [Fact]
        public async Task CancelarVenda_DeveAlterarStatusParaCancelado()
        {
            // Arrange
            var venda = new Venda
            {
                NumeroVenda = "12345",
                ClienteId = 1,
                NomeCliente = "Cliente Teste",
                NomeFilial = "Filial Teste",
                DataVenda = DateTime.UtcNow,
                Itens = new List<ItemVenda>
                {
                    new ItemVenda { ProdutoId = 1, DescricaoProduto = "Produto 1", Quantidade = 2, ValorUnitario = 100 }
                },
                ValorTotal = 200,
                Cancelado = false
            };

            _dbContext.Vendas.Add(venda);
            await _dbContext.SaveChangesAsync();

            // Act
            venda.Cancelado = true;
            _dbContext.Vendas.Update(venda);
            await _dbContext.SaveChangesAsync();

            // Assert
            var vendaCancelada = await _dbContext.Vendas.FirstOrDefaultAsync(v => v.NumeroVenda == "12345");
            vendaCancelada.Cancelado.Should().BeTrue();
        }
    }
}
