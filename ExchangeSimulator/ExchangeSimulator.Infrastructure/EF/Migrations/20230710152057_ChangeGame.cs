using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeSimulator.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Games");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndGame",
                table: "Games",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Games",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndGame",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Games");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "Games",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
