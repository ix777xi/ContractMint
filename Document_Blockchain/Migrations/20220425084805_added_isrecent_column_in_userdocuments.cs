using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Document_Blockchain.Migrations
{
    public partial class added_isrecent_column_in_userdocuments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_recent",
                table: "user_documents",
                type: "tinyint(1)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_recent",
                table: "user_documents");
        }
    }
}
