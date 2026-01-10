using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSoftContable.Migrations
{
    /// <inheritdoc />
    public partial class Reprlegal1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colegios_TipoIdentificacion_TipoIdentificacionId",
                table: "Colegios");

            migrationBuilder.DropIndex(
                name: "IX_Colegios_Cedula",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "Cedula",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "Identificacion",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "RepresentanteLegal",
                table: "Colegios");

            migrationBuilder.AlterColumn<int>(
                name: "TipoIdentificacionId",
                table: "Colegios",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "RepresentantesLegales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroIdentificacion = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TipoIdentificacionId = table.Column<int>(type: "int", nullable: false),
                    ColegioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepresentantesLegales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepresentantesLegales_Colegios_ColegioId",
                        column: x => x.ColegioId,
                        principalTable: "Colegios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepresentantesLegales_TipoIdentificacion_TipoIdentificacionId",
                        column: x => x.TipoIdentificacionId,
                        principalTable: "TipoIdentificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepresentantesLegales_ColegioId",
                table: "RepresentantesLegales",
                column: "ColegioId");

            migrationBuilder.CreateIndex(
                name: "IX_RepresentantesLegales_NumeroIdentificacion",
                table: "RepresentantesLegales",
                column: "NumeroIdentificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RepresentantesLegales_TipoIdentificacionId",
                table: "RepresentantesLegales",
                column: "TipoIdentificacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Colegios_TipoIdentificacion_TipoIdentificacionId",
                table: "Colegios",
                column: "TipoIdentificacionId",
                principalTable: "TipoIdentificacion",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colegios_TipoIdentificacion_TipoIdentificacionId",
                table: "Colegios");

            migrationBuilder.DropTable(
                name: "RepresentantesLegales");

            migrationBuilder.AlterColumn<int>(
                name: "TipoIdentificacionId",
                table: "Colegios",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cedula",
                table: "Colegios",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Identificacion",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RepresentanteLegal",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Colegios_Cedula",
                table: "Colegios",
                column: "Cedula",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Colegios_TipoIdentificacion_TipoIdentificacionId",
                table: "Colegios",
                column: "TipoIdentificacionId",
                principalTable: "TipoIdentificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
