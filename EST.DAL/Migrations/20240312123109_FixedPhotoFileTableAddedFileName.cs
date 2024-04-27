using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EST.DAL.Migrations
{
    public partial class FixedPhotoFileTableAddedFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "PhotoFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "PhotoFiles");
        }
    }
}
