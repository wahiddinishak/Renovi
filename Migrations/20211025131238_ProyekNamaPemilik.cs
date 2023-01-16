using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class ProyekNamaPemilik : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "namaKlien",
                table: "proyeks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "namaKlien",
                table: "proyeks");
        }
    }
}
