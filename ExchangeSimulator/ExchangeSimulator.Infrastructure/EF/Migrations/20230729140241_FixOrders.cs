using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeSimulator.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class FixOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_PlayerCoinId",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PlayerCoinId",
                table: "Orders",
                column: "PlayerCoinId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_PlayerCoinId",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PlayerCoinId",
                table: "Orders",
                column: "PlayerCoinId",
                unique: true);
        }
    }
}
