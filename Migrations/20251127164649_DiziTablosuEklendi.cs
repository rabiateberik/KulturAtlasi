using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KulturAtlasi.Migrations
{
    /// <inheritdoc />
    public partial class DiziTablosuEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diziler",
                columns: table => new
                {
                    DiziID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Yonetmen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaslangicYili = table.Column<int>(type: "int", nullable: true),
                    SezonSayisi = table.Column<int>(type: "int", nullable: true),
                    BolumSayisi = table.Column<int>(type: "int", nullable: true),
                    PosterYolu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KullaniciPuani = table.Column<int>(type: "int", nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurumID = table.Column<int>(type: "int", nullable: false),
                    KullaniciID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diziler", x => x.DiziID);
                    table.ForeignKey(
                        name: "FK_Diziler_AspNetUsers_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Diziler_Durumlar_DurumID",
                        column: x => x.DurumID,
                        principalTable: "Durumlar",
                        principalColumn: "DurumID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diziler_DurumID",
                table: "Diziler",
                column: "DurumID");

            migrationBuilder.CreateIndex(
                name: "IX_Diziler_KullaniciID",
                table: "Diziler",
                column: "KullaniciID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diziler");
        }
    }
}
