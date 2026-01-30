using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KulturAtlasi.Migrations
{
    /// <inheritdoc />
    public partial class SeyahatResimEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResimYolu",
                table: "Seyahatler",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResimYolu",
                table: "Seyahatler");
        }
    }
}
