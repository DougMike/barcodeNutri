using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutritionApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Barcode = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Brand = table.Column<string>(type: "TEXT", nullable: true),
                    Calories = table.Column<double>(type: "REAL", nullable: true),
                    Proteins = table.Column<double>(type: "REAL", nullable: true),
                    Carbohydrates = table.Column<double>(type: "REAL", nullable: true),
                    Fats = table.Column<double>(type: "REAL", nullable: true),
                    RawJson = table.Column<string>(type: "TEXT", nullable: true),
                    RetrievedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Barcode);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
