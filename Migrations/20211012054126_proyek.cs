using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class proyek : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "telepon",
                table: "Tukangs",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nama",
                table: "Tukangs",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "uom",
                table: "Items",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nama",
                table: "Items",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "actDs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreateBy = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    idProyek = table.Column<string>(nullable: true),
                    kegiatan = table.Column<string>(nullable: true),
                    volume = table.Column<double>(nullable: false),
                    uom = table.Column<string>(nullable: true),
                    harga = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actDs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_actDs_Users_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_actDs_Users_UpdateBy",
                        column: x => x.UpdateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "actHs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreateBy = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    idProyek = table.Column<string>(nullable: true),
                    kegiatan = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actHs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_actHs_Users_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_actHs_Users_UpdateBy",
                        column: x => x.UpdateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "materials",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreateBy = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    idProyek = table.Column<string>(nullable: true),
                    idAct = table.Column<int>(nullable: false),
                    item = table.Column<string>(nullable: true),
                    uom = table.Column<string>(nullable: true),
                    harga = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_materials_Users_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_materials_Users_UpdateBy",
                        column: x => x.UpdateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "proyeks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreateBy = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    idProyek = table.Column<string>(nullable: true),
                    judul = table.Column<string>(nullable: true),
                    keterangan = table.Column<string>(nullable: true),
                    mandor = table.Column<int>(nullable: false),
                    tglMulai = table.Column<DateTime>(nullable: false),
                    tglSelesai = table.Column<DateTime>(nullable: false),
                    kontrak = table.Column<string>(nullable: true),
                    pembayaran = table.Column<string>(nullable: true),
                    desain = table.Column<string>(nullable: true),
                    budget = table.Column<double>(nullable: false),
                    actual = table.Column<double>(nullable: false),
                    isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proyeks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_proyeks_Users_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_proyeks_Users_UpdateBy",
                        column: x => x.UpdateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tagihans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreateBy = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    idProyek = table.Column<string>(nullable: true),
                    Seq = table.Column<int>(nullable: false),
                    tglBayar = table.Column<DateTime>(nullable: false),
                    nominal = table.Column<double>(nullable: false),
                    isPaid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tagihans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tagihans_Users_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tagihans_Users_UpdateBy",
                        column: x => x.UpdateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_actDs_CreateBy",
                table: "actDs",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_actDs_UpdateBy",
                table: "actDs",
                column: "UpdateBy");

            migrationBuilder.CreateIndex(
                name: "IX_actHs_CreateBy",
                table: "actHs",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_actHs_UpdateBy",
                table: "actHs",
                column: "UpdateBy");

            migrationBuilder.CreateIndex(
                name: "IX_materials_CreateBy",
                table: "materials",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_materials_UpdateBy",
                table: "materials",
                column: "UpdateBy");

            migrationBuilder.CreateIndex(
                name: "IX_proyeks_CreateBy",
                table: "proyeks",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_proyeks_UpdateBy",
                table: "proyeks",
                column: "UpdateBy");

            migrationBuilder.CreateIndex(
                name: "IX_tagihans_CreateBy",
                table: "tagihans",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_tagihans_UpdateBy",
                table: "tagihans",
                column: "UpdateBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "actDs");

            migrationBuilder.DropTable(
                name: "actHs");

            migrationBuilder.DropTable(
                name: "materials");

            migrationBuilder.DropTable(
                name: "proyeks");

            migrationBuilder.DropTable(
                name: "tagihans");

            migrationBuilder.AlterColumn<string>(
                name: "telepon",
                table: "Tukangs",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "nama",
                table: "Tukangs",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "uom",
                table: "Items",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "nama",
                table: "Items",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
