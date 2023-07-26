using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeSimulator.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddUserReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Review",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Review",
                table: "Users");
        }
    }
}
