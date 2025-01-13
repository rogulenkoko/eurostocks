using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EuroStocks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImageStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "product_image",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "product_image");
        }
    }
}
