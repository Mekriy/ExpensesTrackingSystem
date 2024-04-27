using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EST.DAL.Migrations
{
    public partial class ModifiedItemsModelByAddingRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_UserId",
                table: "Items",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Users_UserId",
                table: "Items",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Users_UserId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_UserId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Items");
        }
    }
}
