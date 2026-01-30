using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KulturAtlasi.Migrations
{
    /// <inheritdoc />
    public partial class EtkilesimTablolari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Metin",
                table: "Yorumlar");

            migrationBuilder.DropColumn(
                name: "BegeniTarihi",
                table: "Begeniler");

            migrationBuilder.RenameColumn(
                name: "KayitTarihi",
                table: "Yorumlar",
                newName: "Tarih");

            migrationBuilder.AddColumn<string>(
                name: "Icerik",
                table: "Yorumlar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icerik",
                table: "Yorumlar");

            migrationBuilder.RenameColumn(
                name: "Tarih",
                table: "Yorumlar",
                newName: "KayitTarihi");

            migrationBuilder.AddColumn<string>(
                name: "Metin",
                table: "Yorumlar",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "BegeniTarihi",
                table: "Begeniler",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
