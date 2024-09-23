using Xunit;
using FluentAssertions;
using NSubstitute;
using _123Vendas.API.Controllers;
using _123Vendas.Core.Business;
using _123Vendas.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using Bogus;

namespace _123Vendas.UnitTests.ControllerTests
{
    public class VendaControllerTests
    {
        private readonly VendaController _controller;
        private readonly IVendaBusiness _vendaBusiness = Substitute.For<IVendaBusiness>();

        public VendaControllerTests()
        {
            _controller = new VendaController(_vendaBusiness);
        }

        [Fact]
        public async Task CriarVenda_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var vendaDto = new Faker<VendaDto>()
                .RuleFor(v => v.Id, f => 1)
                .RuleFor(v => v.NumeroVenda, f => f.Random.String2(5, "0123456789"))
                .RuleFor(v => v.ClienteId, f => f.Random.Int(1, 100))
                .RuleFor(v => v.NomeCliente, f => f.Person.FullName)
                .Generate();

            _vendaBusiness.CriarVendaAsync(vendaDto).Returns(vendaDto);

            // Act
            var result = await _controller.CriarVenda(vendaDto);

            // Assert
            var actionResult = result as CreatedAtActionResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task GetVenda_DeveRetornarOk()
        {
            // Arrange
            var vendaDto = new Faker<VendaDto>()
                .RuleFor(v => v.Id, f => 1)
                .RuleFor(v => v.NumeroVenda, f => f.Random.String2(5, "0123456789"))
                .Generate();

            _vendaBusiness.GetByIdAsync(1).Returns(vendaDto);

            // Act
            var result = await _controller.GetVenda(1);

            // Assert
            var actionResult = result as OkObjectResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetVenda_DeveRetornarNotFound_SeNaoExistir()
        {
            // Arrange
            _vendaBusiness.GetByIdAsync(Arg.Any<int>()).Returns((VendaDto)null);

            // Act
            var result = await _controller.GetVenda(1);

            // Assert
            var actionResult = result as NotFoundObjectResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task AtualizarVenda_DeveRetornarNoContent()
        {
            // Arrange
            var vendaDto = new Faker<VendaDto>()
                .RuleFor(v => v.Id, f => 1)
                .RuleFor(v => v.NumeroVenda, f => f.Random.String2(5, "0123456789"))
                .Generate();

            // Act
            var result = await _controller.AtualizarVenda(1, vendaDto);

            // Assert
            var actionResult = result as NoContentResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task CancelarVenda_DeveRetornarNoContent()
        {
            // Act
            var result = await _controller.CancelarVenda(1);

            // Assert
            var actionResult = result as NoContentResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(204);
        }
    }
}
