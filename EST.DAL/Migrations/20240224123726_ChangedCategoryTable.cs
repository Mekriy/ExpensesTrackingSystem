using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EST.DAL.Migrations
{
    public partial class ChangedCategoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Categories");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Visibility",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
