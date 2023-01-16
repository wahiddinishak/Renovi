using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class personilEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tukang_Users_CreateBy",
                table: "tukang");

            migrationBuilder.DropForeignKey(
                name: "FK_tukang_Users_UpdateBy",
                table: "tukang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tukang",
                table: "tukang");

            migrationBuilder.RenameTable(
                name: "tukang",
                newName: "Tukangs");

            migrationBuilder.RenameIndex(
                name: "IX_tukang_UpdateBy",
                table: "Tukangs",
                newName: "IX_Tukangs_UpdateBy");

            migrationBuilder.RenameIndex(
                name: "IX_tukang_CreateBy",
                table: "Tukangs",
                newName: "IX_Tukangs_CreateBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tukangs",
                table: "Tukangs",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "personils",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreateBy = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    idProyek = table.Column<string>(nullable: true),
                    idTukang = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personils", x => x.Id);
                    table.ForeignKey(
                        name: "FK_personils_Users_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_personils_Users_UpdateBy",
                        column: x => x.UpdateBy,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_personils_CreateBy",
                table: "personils",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_personils_UpdateBy",
                table: "personils",
                column: "UpdateBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Tukangs_Users_CreateBy",
                table: "Tukangs",
                column: "CreateBy",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tukangs_Users_UpdateBy",
                table: "Tukangs",
                column: "UpdateBy",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tukangs_Users_CreateBy",
                table: "Tukangs");

            migrationBuilder.DropForeignKey(
                name: "FK_Tukangs_Users_UpdateBy",
                table: "Tukangs");

            migrationBuilder.DropTable(
                name: "personils");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tukangs",
                table: "Tukangs");

            migrationBuilder.RenameTable(
                name: "Tukangs",
                newName: "tukang");

            migrationBuilder.RenameIndex(
                name: "IX_Tukangs_UpdateBy",
                table: "tukang",
                newName: "IX_tukang_UpdateBy");

            migrationBuilder.RenameIndex(
                name: "IX_Tukangs_CreateBy",
                table: "tukang",
                newName: "IX_tukang_CreateBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tukang",
                table: "tukang",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tukang_Users_CreateBy",
                table: "tukang",
                column: "CreateBy",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tukang_Users_UpdateBy",
                table: "tukang",
                column: "UpdateBy",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
