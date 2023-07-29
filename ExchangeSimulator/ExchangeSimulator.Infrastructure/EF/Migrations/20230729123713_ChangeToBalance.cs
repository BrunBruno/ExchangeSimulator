using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeSimulator.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "StartingCoins",
                newName: "TotalBalance");

            migrationBuilder.RenameColumn(
                name: "Money",
                table: "Players",
                newName: "TotalBalance");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "PlayerCoins",
                newName: "TotalBalance");

            migrationBuilder.RenameColumn(
                name: "NumberOfPlayers",
                table: "Games",
                newName: "TotalPlayers");

            migrationBuilder.RenameColumn(
                name: "Money",
                table: "Games",
                newName: "StartingBalance");

            migrationBuilder.AddColumn<decimal>(
                name: "LockedBalace",
                table: "StartingCoins",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LockedBalance",
                table: "Players",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LockedBalace",
                table: "PlayerCoins",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LockedBalace",
                table: "StartingCoins");

            migrationBuilder.DropColumn(
                name: "LockedBalance",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LockedBalace",
                table: "PlayerCoins");

            migrationBuilder.RenameColumn(
                name: "TotalBalance",
                table: "StartingCoins",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "TotalBalance",
                table: "Players",
                newName: "Money");

            migrationBuilder.RenameColumn(
                name: "TotalBalance",
                table: "PlayerCoins",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "TotalPlayers",
                table: "Games",
                newName: "NumberOfPlayers");

            migrationBuilder.RenameColumn(
                name: "StartingBalance",
                table: "Games",
                newName: "Money");
        }
    }
}
