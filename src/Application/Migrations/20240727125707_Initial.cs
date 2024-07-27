using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGSX.Exploria.Application.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeatherReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    Data = table.Column<byte[]>(type: "VARBINARY(MAX)", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "DATETIME2(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherReport", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dbo_WeatherReport(ReportDate)",
                table: "WeatherReport",
                column: "ReportDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherReport");
        }
    }
}
