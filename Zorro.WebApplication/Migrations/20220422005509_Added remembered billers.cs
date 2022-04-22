using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zorro.WebApplication.Migrations
{
    public partial class Addedrememberedbillers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RememberedBillers",
                columns: table => new
                {
                    BillerCode = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RememberedBillers", x => new { x.BillerCode, x.ApplicationUserId });
                    table.ForeignKey(
                        name: "FK_RememberedBillers_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RememberedBillers_Payees_BillerCode",
                        column: x => x.BillerCode,
                        principalTable: "Payees",
                        principalColumn: "BillerCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RememberedBillers_ApplicationUserId",
                table: "RememberedBillers",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RememberedBillers");

            migrationBuilder.CreateTable(
                name: "DepositRequestDto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CCExpiry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditCardNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositRequestDto", x => x.Id);
                });
        }
    }
}
