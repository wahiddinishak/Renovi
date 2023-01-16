using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class tukangEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tukangs_Users_CreateBy",
                table: "Tukangs");

            migrationBuilder.DropForeignKey(
                name: "FK_Tukangs_Users_UpdateBy",
                table: "Tukangs");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
