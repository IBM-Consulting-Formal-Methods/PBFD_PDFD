using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitorLog_PDFD.Migrations
{
    /// <inheritdoc />
    public partial class FixCityandStateNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Counties",
                keyColumn: "CountyId",
                keyValue: 2,
                column: "Name",
                value: "Baltimore");

            migrationBuilder.UpdateData(
                table: "States",
                keyColumn: "StateId",
                keyValue: 2,
                column: "Name",
                value: "Virginia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Counties",
                keyColumn: "CountyId",
                keyValue: 2,
                column: "Name",
                value: "Boltimore");

            migrationBuilder.UpdateData(
                table: "States",
                keyColumn: "StateId",
                keyValue: 2,
                column: "Name",
                value: "Viginia");
        }
    }
}
