using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _123Vendas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCancelProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Cancelado",
                table: "ItensVenda",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cancelado",
                table: "ItensVenda");
        }
    }
}
