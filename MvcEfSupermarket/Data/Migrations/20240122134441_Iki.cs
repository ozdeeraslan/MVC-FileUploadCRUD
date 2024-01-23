using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcEfSupermarket.Data.Migrations
{
    /// <inheritdoc />
    public partial class Iki : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResimAdi",
                table: "Urunler",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResimAdi",
                table: "Urunler");
        }
    }
}
