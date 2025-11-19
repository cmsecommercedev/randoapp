using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace randevuappapi.Migrations
{
    /// <inheritdoc />
    public partial class Adddfhdfıı : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessPhoto_Businesses_BusinessId",
                table: "BusinessPhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessPhoto",
                table: "BusinessPhoto");

            migrationBuilder.RenameTable(
                name: "BusinessPhoto",
                newName: "BusinessPhotos");

            migrationBuilder.RenameIndex(
                name: "IX_BusinessPhoto_BusinessId",
                table: "BusinessPhotos",
                newName: "IX_BusinessPhotos_BusinessId");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusinessPhotos",
                table: "BusinessPhotos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessPhotos_Businesses_BusinessId",
                table: "BusinessPhotos",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessPhotos_Businesses_BusinessId",
                table: "BusinessPhotos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessPhotos",
                table: "BusinessPhotos");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "BusinessPhotos",
                newName: "BusinessPhoto");

            migrationBuilder.RenameIndex(
                name: "IX_BusinessPhotos_BusinessId",
                table: "BusinessPhoto",
                newName: "IX_BusinessPhoto_BusinessId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusinessPhoto",
                table: "BusinessPhoto",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessPhoto_Businesses_BusinessId",
                table: "BusinessPhoto",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
