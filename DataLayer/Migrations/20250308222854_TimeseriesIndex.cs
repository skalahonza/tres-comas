using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class TimeseriesIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CarbsValues_Time",
                table: "CarbsValues",
                column: "Time");

            migrationBuilder.CreateIndex(
                name: "IX_BolusValues_Time",
                table: "BolusValues",
                column: "Time");

            migrationBuilder.CreateIndex(
                name: "IX_BgValues_Time",
                table: "BgValues",
                column: "Time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarbsValues_Time",
                table: "CarbsValues");

            migrationBuilder.DropIndex(
                name: "IX_BolusValues_Time",
                table: "BolusValues");

            migrationBuilder.DropIndex(
                name: "IX_BgValues_Time",
                table: "BgValues");
        }
    }
}
