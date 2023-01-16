using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class LaporanEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "laporanAttachments",
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
                    jenis = table.Column<string>(nullable: true),
                    filename = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_laporanAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_laporanAttachments_Users_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_laporanAttachments_Users_UpdateBy",
                        column: x => x.UpdateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "laporans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreateBy = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    idProyek = table.Column<string>(nullable: true),
                    reportDate = table.Column<DateTime>(nullable: false),
                    idActD = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_laporans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_laporans_Users_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_laporans_Users_UpdateBy",
                        column: x => x.UpdateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "laporanUsages",
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
                    idMaterial = table.Column<int>(nullable: false),
                    qty = table.Column<double>(nullable: false),
                    amount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_laporanUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_laporanUsages_Users_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_laporanUsages_Users_UpdateBy",
                        column: x => x.UpdateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_laporanAttachments_CreateBy",
                table: "laporanAttachments",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_laporanAttachments_UpdateBy",
                table: "laporanAttachments",
                column: "UpdateBy");

            migrationBuilder.CreateIndex(
                name: "IX_laporans_CreateBy",
                table: "laporans",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_laporans_UpdateBy",
                table: "laporans",
                column: "UpdateBy");

            migrationBuilder.CreateIndex(
                name: "IX_laporanUsages_CreateBy",
                table: "laporanUsages",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_laporanUsages_UpdateBy",
                table: "laporanUsages",
                column: "UpdateBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "laporanAttachments");

            migrationBuilder.DropTable(
                name: "laporans");

            migrationBuilder.DropTable(
                name: "laporanUsages");
        }
    }
}
