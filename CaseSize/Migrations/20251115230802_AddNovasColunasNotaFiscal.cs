using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaseSize.Migrations
{
    /// <inheritdoc />
    public partial class AddNovasColunasNotaFiscal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Desagio",
                table: "NotasFiscais",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorLiquido",
                table: "NotasFiscais",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Desagio",
                table: "NotasFiscais");

            migrationBuilder.DropColumn(
                name: "ValorLiquido",
                table: "NotasFiscais");
        }
    }
}
