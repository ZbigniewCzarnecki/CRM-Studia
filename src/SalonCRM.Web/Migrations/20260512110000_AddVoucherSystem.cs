using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SalonCRM.Web.Data;

#nullable disable

namespace SalonCRM.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260512110000_AddVoucherSystem")]
    public partial class AddVoucherSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VoucherId",
                table: "Appointments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RecipientName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, defaultValue: ""),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    AmountValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    ServiceName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, defaultValue: ""),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsUsed = table.Column<bool>(type: "INTEGER", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UsedInAppointment = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false, defaultValue: ""),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, defaultValue: ""),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AppointmentId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoucherId",
                table: "Appointments");

            migrationBuilder.DropTable(
                name: "Vouchers");
        }
    }
}
