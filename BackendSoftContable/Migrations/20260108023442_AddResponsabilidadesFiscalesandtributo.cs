using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSoftContable.Migrations
{
    /// <inheritdoc />
    public partial class AddResponsabilidadesFiscalesandtributo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tributo_Colegios_ColegioId",
                table: "Tributo");

            migrationBuilder.DropTable(
                name: "ColegioResponsabilidadFiscal");

            migrationBuilder.DropIndex(
                name: "IX_Tributo_ColegioId",
                table: "Tributo");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Tributo");

            migrationBuilder.DropColumn(
                name: "ColegioId",
                table: "Tributo");

            migrationBuilder.DropColumn(
                name: "FechaAsignacion",
                table: "ResponsabilidadFiscal");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Tributo",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ResponsabilidadFiscalId",
                table: "Colegios",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "TributoId",
                table: "Colegios",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Colegios_ResponsabilidadFiscalId",
                table: "Colegios",
                column: "ResponsabilidadFiscalId");

            migrationBuilder.CreateIndex(
                name: "IX_Colegios_TributoId",
                table: "Colegios",
                column: "TributoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Colegios_ResponsabilidadFiscal_ResponsabilidadFiscalId",
                table: "Colegios",
                column: "ResponsabilidadFiscalId",
                principalTable: "ResponsabilidadFiscal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Colegios_Tributo_TributoId",
                table: "Colegios",
                column: "TributoId",
                principalTable: "Tributo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
        INSERT INTO Tributo (Nombre, Descripcion) VALUES
        ('IVA', 'Impuesto al Valor Agregado'),
        ('ICA', 'Impuesto de Industria y Comercio'),
        ('Retención en la Fuente', 'Retención de impuestos a proveedores'),
        ('Impuesto de Timbre', 'Aplicable a documentos legales'),
        ('ReteICA', 'Retención de ICA a terceros');
    ");

            migrationBuilder.Sql(@"
        INSERT INTO ResponsabilidadFiscal (Nombre, Descripcion, ImpuestosRelacionados, PeriodicidadDeclaracion) VALUES
        ('Gran Contribuyente', 'Contribuyente de gran tamaño', 'Renta, IVA', 'Mensual'),
        ('Autorretenedor', 'Autorretenedor de impuestos', 'Renta, IVA', 'Mensual'),
        ('Agente de Retención IVA', 'Retención de IVA a terceros', 'IVA', 'Mensual'),
        ('Régimen simple de tributación', 'Pequeños contribuyentes', 'Renta, IVA', 'Anual'),
        ('No aplica - Otros', 'Sin responsabilidades especiales', '', 'Anual');
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colegios_ResponsabilidadFiscal_ResponsabilidadFiscalId",
                table: "Colegios");

            migrationBuilder.DropForeignKey(
                name: "FK_Colegios_Tributo_TributoId",
                table: "Colegios");

            migrationBuilder.DropIndex(
                name: "IX_Colegios_ResponsabilidadFiscalId",
                table: "Colegios");

            migrationBuilder.DropIndex(
                name: "IX_Colegios_TributoId",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "ResponsabilidadFiscalId",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "TributoId",
                table: "Colegios");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Tributo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Tributo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ColegioId",
                table: "Tributo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAsignacion",
                table: "ResponsabilidadFiscal",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
                name: "IX_Tributo_ColegioId",
                table: "Tributo",
                column: "ColegioId");

            migrationBuilder.CreateIndex(
                name: "IX_ColegioResponsabilidadFiscal_ResponsabilidadesFiscalesId",
                table: "ColegioResponsabilidadFiscal",
                column: "ResponsabilidadesFiscalesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tributo_Colegios_ColegioId",
                table: "Tributo",
                column: "ColegioId",
                principalTable: "Colegios",
                principalColumn: "Id");
        }
    }
}
