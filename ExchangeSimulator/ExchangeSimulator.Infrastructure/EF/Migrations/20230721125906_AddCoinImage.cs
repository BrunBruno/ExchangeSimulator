using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeSimulator.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddCoinImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "StartingCoins",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "PlayerCoins",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "StartingCoins");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "PlayerCoins");
        }
    }
}
