using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIDevSteam.Migrations
{
    /// <inheritdoc />
    public partial class Preco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataFinazalizacao",
                table: "Carrinhos",
                newName: "DataFinalizacao");

            migrationBuilder.AlterColumn<string>(
                name: "Banner",
                table: "Jogos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoOriginal",
                table: "Jogos",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecoOriginal",
                table: "Jogos");

            migrationBuilder.RenameColumn(
                name: "DataFinalizacao",
                table: "Carrinhos",
                newName: "DataFinazalizacao");

            migrationBuilder.AlterColumn<string>(
                name: "Banner",
                table: "Jogos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
