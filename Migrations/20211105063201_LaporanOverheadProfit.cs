using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class LaporanOverheadProfit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "laporanOverheadProfits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreateBy = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    idProyek = table.Column<string>(nullable: true),
                    header = table.Column<int>(nullable: false),
                    amount = table.Column<double>(nullable: false),
                    info = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_laporanOverheadProfits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_laporanOverheadProfits_Users_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_laporanOverheadProfits_Users_UpdateBy",
                        column: x => x.UpdateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_laporanOverheadProfits_CreateBy",
                table: "laporanOverheadProfits",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_laporanOverheadProfits_UpdateBy",
                table: "laporanOverheadProfits",
                column: "UpdateBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "laporanOverheadProfits");
        }
    }
}
