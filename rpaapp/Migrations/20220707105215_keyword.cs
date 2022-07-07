using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rpaapp.Migrations
{
    public partial class keyword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Keyword",
                table: "Firms",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Keyword",
                table: "Firms");
        }
    }
}
