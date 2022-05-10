using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zorro.Dal.Migrations
{
    public partial class Merchant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BusinessName",
                table: "Merchants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ACCPosition",
                table: "Merchants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyPhone",
                table: "Merchants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateEstablished",
                table: "Merchants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriversLicense",
                table: "Merchants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Merchants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Merchants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TradingAddress",
                table: "Merchants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TradingName",
                table: "Merchants",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ACCPosition",
                table: "Merchants");

            migrationBuilder.DropColumn(
                name: "CompanyPhone",
                table: "Merchants");

            migrationBuilder.DropColumn(
                name: "DateEstablished",
                table: "Merchants");

            migrationBuilder.DropColumn(
                name: "DriversLicense",
                table: "Merchants");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Merchants");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Merchants");

            migrationBuilder.DropColumn(
                name: "TradingAddress",
                table: "Merchants");

            migrationBuilder.DropColumn(
                name: "TradingName",
                table: "Merchants");

            migrationBuilder.AlterColumn<string>(
                name: "BusinessName",
                table: "Merchants",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
