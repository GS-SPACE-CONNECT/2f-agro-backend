using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiapAgro.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alertas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PropriedadeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Probabilidade = table.Column<double>(type: "double precision", nullable: false),
                    tipo_alerta = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    VolumeMM = table.Column<double>(type: "double precision", nullable: true),
                    InclinacaoSolo = table.Column<double>(type: "double precision", nullable: true),
                    TemperaturaMinima = table.Column<double>(type: "double precision", nullable: true),
                    EspeciePraga = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CulturaAfetada = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DiasSemChuva = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alertas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "propriedades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Municipio = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Estado = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    AreaHectares = table.Column<double>(type: "double precision", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    localizacao_lat = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    localizacao_lng = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_propriedades", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alertas");

            migrationBuilder.DropTable(
                name: "propriedades");
        }
    }
}
