using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SalonCRM.Web.Data;

#nullable disable

namespace SalonCRM.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260512120000_AddVoucherClientAndBalance")]
    public partial class AddVoucherClientAndBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "VoucherDeduction",
                table: "Appointments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Vouchers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingAmount",
                table: "Vouchers",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "VoucherDeduction", table: "Appointments");
            migrationBuilder.DropColumn(name: "ClientId", table: "Vouchers");
            migrationBuilder.DropColumn(name: "RemainingAmount", table: "Vouchers");
        }
    }
}
