using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace randevuappapi.Migrations
{
    /// <inheritdoc />
    public partial class AddStoryImage555dfsdff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoUrlsJson",
                table: "Businesses",
                newName: "MainPhotoUrl");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Businesses",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Businesses",
                type: "float",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BusinessPhoto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessPhoto_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPhoto_BusinessId",
                table: "BusinessPhoto",
                column: "BusinessId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessPhoto");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Businesses");

            migrationBuilder.RenameColumn(
                name: "MainPhotoUrl",
                table: "Businesses",
                newName: "PhotoUrlsJson");
        }
    }
}
