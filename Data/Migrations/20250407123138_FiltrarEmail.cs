using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticaSupervisada.Data.Migrations
{
    /// <inheritdoc />
    public partial class FiltrarEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "Bidones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "Asistencias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "Bidones");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "Asistencias");
        }
    }
}
