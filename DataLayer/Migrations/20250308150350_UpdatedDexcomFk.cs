using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDexcomFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DexcomUserSettings_Users_UserId1",
                table: "DexcomUserSettings");

            migrationBuilder.DropIndex(
                name: "IX_DexcomUserSettings_UserId1",
                table: "DexcomUserSettings");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "DexcomUserSettings");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "DexcomUserSettings",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_DexcomUserSettings_UserId",
                table: "DexcomUserSettings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DexcomUserSettings_Users_UserId",
                table: "DexcomUserSettings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DexcomUserSettings_Users_UserId",
                table: "DexcomUserSettings");

            migrationBuilder.DropIndex(
                name: "IX_DexcomUserSettings_UserId",
                table: "DexcomUserSettings");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "DexcomUserSettings",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "DexcomUserSettings",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DexcomUserSettings_UserId1",
                table: "DexcomUserSettings",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DexcomUserSettings_Users_UserId1",
                table: "DexcomUserSettings",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
