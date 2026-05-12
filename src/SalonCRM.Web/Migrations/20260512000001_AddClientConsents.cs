using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonCRM.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddClientConsents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConsentEmail",
                table: "Clients",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ConsentSms",
                table: "Clients",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsentEmail",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ConsentSms",
                table: "Clients");
        }
    }
}
