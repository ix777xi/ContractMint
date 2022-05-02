using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Document_Blockchain.Migrations
{
    public partial class added_userdocumentid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "user_document_id",
                table: "hash_details",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "user_document_id",
                table: "envelope_detail",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_hash_details_user_document_id",
                table: "hash_details",
                column: "user_document_id");

            migrationBuilder.CreateIndex(
                name: "IX_envelope_detail_user_document_id",
                table: "envelope_detail",
                column: "user_document_id");

            migrationBuilder.AddForeignKey(
                name: "FK_envelope_detail_user_documents_user_document_id",
                table: "envelope_detail",
                column: "user_document_id",
                principalTable: "user_documents",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_hash_details_user_documents_user_document_id",
                table: "hash_details",
                column: "user_document_id",
                principalTable: "user_documents",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_envelope_detail_user_documents_user_document_id",
                table: "envelope_detail");

            migrationBuilder.DropForeignKey(
                name: "FK_hash_details_user_documents_user_document_id",
                table: "hash_details");

            migrationBuilder.DropIndex(
                name: "IX_hash_details_user_document_id",
                table: "hash_details");

            migrationBuilder.DropIndex(
                name: "IX_envelope_detail_user_document_id",
                table: "envelope_detail");

            migrationBuilder.DropColumn(
                name: "user_document_id",
                table: "hash_details");

            migrationBuilder.DropColumn(
                name: "user_document_id",
                table: "envelope_detail");
        }
    }
}
