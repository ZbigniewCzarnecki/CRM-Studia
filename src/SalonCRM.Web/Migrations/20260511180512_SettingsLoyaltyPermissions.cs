using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonCRM.Web.Migrations
{
    /// <inheritdoc />
    public partial class SettingsLoyaltyPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoyaltyStamps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    AwardedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AppointmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    AddedByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: false),
                    IsManual = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoyaltyStamps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoyaltyStamps_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SalonName = table.Column<string>(type: "TEXT", nullable: false),
                    WorkDayStart = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkDayEnd = table.Column<int>(type: "INTEGER", nullable: false),
                    SlotMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    LoyaltyStampsForReward = table.Column<int>(type: "INTEGER", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CanManageAppointments = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanDeleteAppointments = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanEditClientData = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanManageServices = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanViewReports = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanManageWorkers = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanManageSettings = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanManageLoyaltyCards = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyStamps_ClientId",
                table: "LoyaltyStamps",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoyaltyStamps");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "UserPermissions");
        }
    }
}
