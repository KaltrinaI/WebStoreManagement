using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebStoreApp.Migrations
{
    /// <inheritdoc />
    public partial class FixDiscountTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DisountPercentage",
                table: "Discounts",
                newName: "DiscountPercentage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountPercentage",
                table: "Discounts",
                newName: "DisountPercentage");
        }
    }
}
