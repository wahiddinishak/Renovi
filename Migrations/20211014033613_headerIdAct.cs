using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class headerIdAct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Seq",
                table: "actHs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "header",
                table: "actDs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Seq",
                table: "actHs");

            migrationBuilder.DropColumn(
                name: "header",
                table: "actDs");
        }
    }
}
