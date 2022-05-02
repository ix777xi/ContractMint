using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Document_Blockchain.Migrations
{
    public partial class intdailingcode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "international_dailing_code",
                table: "user",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "international_dailing_code",
                table: "user");
        }
    }
}
