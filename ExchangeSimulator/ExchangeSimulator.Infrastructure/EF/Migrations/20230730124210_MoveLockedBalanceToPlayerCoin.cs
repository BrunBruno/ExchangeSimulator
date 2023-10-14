using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeSimulator.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class MoveLockedBalanceToPlayerCoin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LockedBalace",
                table: "StartingCoins");

            migrationBuilder.RenameColumn(
                name: "LockedBalace",
                table: "PlayerCoins",
                newName: "LockedBalance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LockedBalance",
                table: "PlayerCoins",
                newName: "LockedBalace");

            migrationBuilder.AddColumn<decimal>(
                name: "LockedBalace",
                table: "StartingCoins",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
