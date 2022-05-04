using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zorro.Dal.Migrations
{
    public partial class RefinedMerchantModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Merchants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Merchants");
        }
    }
}
