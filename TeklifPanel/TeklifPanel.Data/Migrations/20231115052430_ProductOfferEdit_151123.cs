using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeklifPanel.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProductOfferEdit_151123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeadLine",
                table: "ProductOffers",
                newName: "Deadline");

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "ProductOffers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "ProductOffers");

            migrationBuilder.RenameColumn(
                name: "Deadline",
                table: "ProductOffers",
                newName: "DeadLine");
        }
    }
}
