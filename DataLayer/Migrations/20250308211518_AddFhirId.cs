using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddFhirId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FhirId",
                table: "CarbsValues",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FhirId",
                table: "BolusValues",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FhirId",
                table: "BgValues",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FhirId",
                table: "CarbsValues");

            migrationBuilder.DropColumn(
                name: "FhirId",
                table: "BolusValues");

            migrationBuilder.DropColumn(
                name: "FhirId",
                table: "BgValues");
        }
    }
}
