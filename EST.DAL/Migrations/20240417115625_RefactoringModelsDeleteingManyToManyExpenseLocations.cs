using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EST.DAL.Migrations
{
    public partial class RefactoringModelsDeleteingManyToManyExpenseLocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "Expenses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql(
                "UPDATE e " +
                "SET " +
                "e.LocationId = l.LocationId " +
                "FROM Expenses e " +
                "INNER JOIN " +
                "ExpensesLocations l " +
                "ON e.Id = l.ExpenseId"
            );
            
            migrationBuilder.CreateIndex(
                name: "IX_Expenses_LocationId",
                table: "Expenses",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Locations_LocationId",
                table: "Expenses",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            
            migrationBuilder.DropTable(
                name: "ExpensesLocations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpensesLocations",
                columns: table => new
                {
                    ExpenseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpensesLocations", x => new { x.ExpenseId, x.LocationId });
                    table.ForeignKey(
                        name: "FK_ExpensesLocations_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpensesLocations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateIndex(
                name: "IX_ExpensesLocations_LocationId",
                table: "ExpensesLocations",
                column: "LocationId");
            
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Locations_LocationId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_LocationId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Expenses");
        }
    }
}
