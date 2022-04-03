using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zorro.WebApplication.Data.Migrations
{
    public partial class RemoveAccountNumFromTransact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Acconuts_AccountNumber",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "AccountNumber",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Acconuts_AccountNumber",
                table: "Transactions",
                column: "AccountNumber",
                principalTable: "Acconuts",
                principalColumn: "AccountNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Acconuts_AccountNumber",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "AccountNumber",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Acconuts_AccountNumber",
                table: "Transactions",
                column: "AccountNumber",
                principalTable: "Acconuts",
                principalColumn: "AccountNumber",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
