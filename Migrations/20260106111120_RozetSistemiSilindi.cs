using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KulturAtlasi.Migrations
{
    /// <inheritdoc />
    public partial class RozetSistemiSilindi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KullaniciRozetleri");

            migrationBuilder.DropTable(
                name: "Rozetler");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rozetler",
                columns: table => new
                {
                    RozetID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ikon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RozetAdi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rozetler", x => x.RozetID);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciRozetleri",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RozetID = table.Column<int>(type: "int", nullable: false),
                    VerilmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciRozetleri", x => x.ID);
                    table.ForeignKey(
                        name: "FK_KullaniciRozetleri_AspNetUsers_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KullaniciRozetleri_Rozetler_RozetID",
                        column: x => x.RozetID,
                        principalTable: "Rozetler",
                        principalColumn: "RozetID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciRozetleri_KullaniciID",
                table: "KullaniciRozetleri",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciRozetleri_RozetID",
                table: "KullaniciRozetleri",
                column: "RozetID");
        }
    }
}
