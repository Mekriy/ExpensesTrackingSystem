using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EST.DAL.Migrations
{
    public partial class AddedUserRelationToLocationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Save",
                table: "Locations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Locations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_UserId",
                table: "Locations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Users_UserId",
                table: "Locations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Users_UserId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_UserId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Save",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Locations");
        }
    }
}
