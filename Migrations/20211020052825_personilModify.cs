using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class personilModify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idTukang",
                table: "personils");

            migrationBuilder.AddColumn<string>(
                name: "akunBank",
                table: "personils",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nama",
                table: "personils",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "telepon",
                table: "personils",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "akunBank",
                table: "personils");

            migrationBuilder.DropColumn(
                name: "nama",
                table: "personils");

            migrationBuilder.DropColumn(
                name: "telepon",
                table: "personils");

            migrationBuilder.AddColumn<int>(
                name: "idTukang",
                table: "personils",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
