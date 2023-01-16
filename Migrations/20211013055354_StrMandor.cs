using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class StrMandor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "actual",
                table: "proyeks");

            migrationBuilder.DropColumn(
                name: "keterangan",
                table: "proyeks");

            migrationBuilder.DropColumn(
                name: "pembayaran",
                table: "proyeks");

            migrationBuilder.AlterColumn<string>(
                name: "mandor",
                table: "proyeks",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "alamat",
                table: "proyeks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pemilik",
                table: "proyeks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "alamat",
                table: "proyeks");

            migrationBuilder.DropColumn(
                name: "pemilik",
                table: "proyeks");

            migrationBuilder.AlterColumn<int>(
                name: "mandor",
                table: "proyeks",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "actual",
                table: "proyeks",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "keterangan",
                table: "proyeks",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pembayaran",
                table: "proyeks",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
