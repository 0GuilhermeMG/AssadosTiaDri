using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPDVAssadosTiaDri.Migrations
{
    /// <inheritdoc />
    public partial class AddCodigoDeBarrasToProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoDeBarras",
                table: "Produtos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoDeBarras",
                table: "Produtos");
        }
    }
}
