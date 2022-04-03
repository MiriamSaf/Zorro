using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zorro.WebApplication.Data.Migrations
{
    public partial class RemoveAccountNumFromTransact4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Acconuts_AccountNumber",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "AccountNumber",
                table: "Transactions",
                newName: "DestinationAccountNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_AccountNumber",
                table: "Transactions",
                newName: "IX_Transactions_DestinationAccountNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Acconuts_DestinationAccountNumber",
                table: "Transactions",
                column: "DestinationAccountNumber",
                principalTable: "Acconuts",
                principalColumn: "AccountNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Acconuts_DestinationAccountNumber",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "DestinationAccountNumber",
                table: "Transactions",
                newName: "AccountNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_DestinationAccountNumber",
                table: "Transactions",
                newName: "IX_Transactions_AccountNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Acconuts_AccountNumber",
                table: "Transactions",
                column: "AccountNumber",
                principalTable: "Acconuts",
                principalColumn: "AccountNumber");
        }
    }
}
