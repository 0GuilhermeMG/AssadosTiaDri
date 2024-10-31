using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPDVAssadosTiaDri.Migrations
{
    /// <inheritdoc />
    public partial class AddVendaEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Vendas",
                newName: "DataVenda");

            migrationBuilder.RenameColumn(
                name: "PrecoUnitario",
                table: "ItensVenda",
                newName: "Preco");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataVenda",
                table: "Vendas",
                newName: "Data");

            migrationBuilder.RenameColumn(
                name: "Preco",
                table: "ItensVenda",
                newName: "PrecoUnitario");
        }
    }
}
