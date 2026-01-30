using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KulturAtlasi.Migrations
{
    /// <inheritdoc />
    public partial class GizlilikAyariEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GizliHesap",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GizliHesap",
                table: "AspNetUsers");
        }
    }
}
