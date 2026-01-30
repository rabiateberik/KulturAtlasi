using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KulturAtlasi.Migrations
{
    /// <inheritdoc />
    public partial class TakipSistemiEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Takipler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TakipEdenID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TakipEdilenID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TakipTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Takipler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Takipler_AspNetUsers_TakipEdenID",
                        column: x => x.TakipEdenID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Takipler_AspNetUsers_TakipEdilenID",
                        column: x => x.TakipEdilenID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Takipler_TakipEdenID",
                table: "Takipler",
                column: "TakipEdenID");

            migrationBuilder.CreateIndex(
                name: "IX_Takipler_TakipEdilenID",
                table: "Takipler",
                column: "TakipEdilenID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Takipler");
        }
    }
}
