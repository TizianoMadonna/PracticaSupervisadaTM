using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticaSupervisada.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asistencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre_Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Tiempo_Entrada = table.Column<TimeOnly>(type: "time", nullable: false),
                    Tiempo_Salida = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencias", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asistencias");
        }
    }
}
