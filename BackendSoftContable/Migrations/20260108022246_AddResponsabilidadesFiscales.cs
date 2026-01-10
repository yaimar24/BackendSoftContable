using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSoftContable.Migrations
{
    /// <inheritdoc />
    public partial class AddResponsabilidadesFiscales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Responsabilidadesfiscales",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "Tributos",
                table: "Colegios");

            migrationBuilder.CreateTable(
                name: "ResponsabilidadFiscal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImpuestosRelacionados = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PeriodicidadDeclaracion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsabilidadFiscal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tributo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColegioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tributo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tributo_Colegios_ColegioId",
                        column: x => x.ColegioId,
                        principalTable: "Colegios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ColegioResponsabilidadFiscal",
                columns: table => new
                {
                    ColegiosId = table.Column<int>(type: "int", nullable: false),
                    ResponsabilidadesFiscalesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColegioResponsabilidadFiscal", x => new { x.ColegiosId, x.ResponsabilidadesFiscalesId });
                    table.ForeignKey(
                        name: "FK_ColegioResponsabilidadFiscal_Colegios_ColegiosId",
                        column: x => x.ColegiosId,
                        principalTable: "Colegios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ColegioResponsabilidadFiscal_ResponsabilidadFiscal_ResponsabilidadesFiscalesId",
                        column: x => x.ResponsabilidadesFiscalesId,
                        principalTable: "ResponsabilidadFiscal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColegioResponsabilidadFiscal_ResponsabilidadesFiscalesId",
                table: "ColegioResponsabilidadFiscal",
                column: "ResponsabilidadesFiscalesId");

            migrationBuilder.CreateIndex(
                name: "IX_Tributo_ColegioId",
                table: "Tributo",
                column: "ColegioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColegioResponsabilidadFiscal");

            migrationBuilder.DropTable(
                name: "Tributo");

            migrationBuilder.DropTable(
                name: "ResponsabilidadFiscal");

            migrationBuilder.AddColumn<string>(
                name: "Responsabilidadesfiscales",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tributos",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
