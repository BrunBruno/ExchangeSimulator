using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeSimulator.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCoin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coin", x => x.Id);
                });
        }
    }
}
