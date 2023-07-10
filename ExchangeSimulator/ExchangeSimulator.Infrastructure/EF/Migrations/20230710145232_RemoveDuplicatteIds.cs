using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeSimulator.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicatteIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCoins_Players_PlayerId1",
                table: "PlayerCoins");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Games_GameId1",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_StartingCoins_Games_GameId1",
                table: "StartingCoins");

            migrationBuilder.DropIndex(
                name: "IX_StartingCoins_GameId1",
                table: "StartingCoins");

            migrationBuilder.DropIndex(
                name: "IX_Players_GameId1",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_PlayerCoins_PlayerId1",
                table: "PlayerCoins");

            migrationBuilder.DropColumn(
                name: "GameId1",
                table: "StartingCoins");

            migrationBuilder.DropColumn(
                name: "GameId1",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PlayerId1",
                table: "PlayerCoins");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GameId1",
                table: "StartingCoins",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GameId1",
                table: "Players",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlayerId1",
                table: "PlayerCoins",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StartingCoins_GameId1",
                table: "StartingCoins",
                column: "GameId1");

            migrationBuilder.CreateIndex(
                name: "IX_Players_GameId1",
                table: "Players",
                column: "GameId1");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCoins_PlayerId1",
                table: "PlayerCoins",
                column: "PlayerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCoins_Players_PlayerId1",
                table: "PlayerCoins",
                column: "PlayerId1",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Games_GameId1",
                table: "Players",
                column: "GameId1",
                principalTable: "Games",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StartingCoins_Games_GameId1",
                table: "StartingCoins",
                column: "GameId1",
                principalTable: "Games",
                principalColumn: "Id");
        }
    }
}
