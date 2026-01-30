using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KulturAtlasi.Migrations
{
    /// <inheritdoc />
    public partial class PaylasimGuncelleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BegeniSayisi",
                table: "Paylasimlar");

            migrationBuilder.DropColumn(
                name: "KaynakTipi",
                table: "Paylasimlar");

            migrationBuilder.DropColumn(
                name: "YorumSayisi",
                table: "Paylasimlar");

            migrationBuilder.RenameColumn(
                name: "PaylasimTarihi",
                table: "Paylasimlar",
                newName: "Tarih");

            migrationBuilder.RenameColumn(
                name: "Metin",
                table: "Paylasimlar",
                newName: "IlgiliIcerikTuru");

            migrationBuilder.RenameColumn(
                name: "KaynakID",
                table: "Paylasimlar",
                newName: "IlgiliIcerikID");

            migrationBuilder.AddColumn<string>(
                name: "Icerik",
                table: "Paylasimlar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaylasimTuru",
                table: "Paylasimlar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icerik",
                table: "Paylasimlar");

            migrationBuilder.DropColumn(
                name: "PaylasimTuru",
                table: "Paylasimlar");

            migrationBuilder.RenameColumn(
                name: "Tarih",
                table: "Paylasimlar",
                newName: "PaylasimTarihi");

            migrationBuilder.RenameColumn(
                name: "IlgiliIcerikTuru",
                table: "Paylasimlar",
                newName: "Metin");

            migrationBuilder.RenameColumn(
                name: "IlgiliIcerikID",
                table: "Paylasimlar",
                newName: "KaynakID");

            migrationBuilder.AddColumn<int>(
                name: "BegeniSayisi",
                table: "Paylasimlar",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "KaynakTipi",
                table: "Paylasimlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YorumSayisi",
                table: "Paylasimlar",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
