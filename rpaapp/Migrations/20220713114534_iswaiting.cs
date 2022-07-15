using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rpaapp.Migrations
{
    public partial class iswaiting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AltVAT",
                table: "Firms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientCode",
                table: "Firms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceType",
                table: "Firms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Firms",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltVAT",
                table: "Firms");

            migrationBuilder.DropColumn(
                name: "ClientCode",
                table: "Firms");

            migrationBuilder.DropColumn(
                name: "InvoiceType",
                table: "Firms");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Firms");
        }
    }
}
