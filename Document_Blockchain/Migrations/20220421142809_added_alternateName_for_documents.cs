using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Document_Blockchain.Migrations
{
    public partial class added_alternateName_for_documents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "alternate_name",
                table: "user_documents",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "alternate_name",
                table: "documents",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "alternate_name",
                table: "user_documents");

            migrationBuilder.DropColumn(
                name: "alternate_name",
                table: "documents");
        }
    }
}
