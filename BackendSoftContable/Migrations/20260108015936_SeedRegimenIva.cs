using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSoftContable.Migrations
{
    /// <inheritdoc />
    public partial class SeedRegimenIva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Usuarios",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nit",
                table: "Colegios",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Cedula",
                table: "Colegios",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Ciudad",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoActividadEconomica",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Identificacion",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IvaRetencion",
                table: "Colegios",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ManejaAiu",
                table: "Colegios",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegimenIvaId",
                table: "Colegios",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Responsabilidadesfiscales",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TarifaIca",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tributos",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RegimenesIva",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegimenesIva", x => x.Id);
                });
            migrationBuilder.InsertData(
    table: "RegimenesIva",
    columns: new[] { "Id", "Nombre", "Codigo", "Descripcion", "Activo" },
    values: new object[,]
    {
        { 1, "Responsable del IVA", "RI", "Responsable del Impuesto sobre las Ventas", true },
        { 2, "No responsable del IVA", "NRI", "No responsable del Impuesto sobre las Ventas", true }
    });
            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colegios_Cedula",
                table: "Colegios",
                column: "Cedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colegios_Nit",
                table: "Colegios",
                column: "Nit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colegios_RegimenIvaId",
                table: "Colegios",
                column: "RegimenIvaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Colegios_RegimenesIva_RegimenIvaId",
                table: "Colegios",
                column: "RegimenIvaId",
                principalTable: "RegimenesIva",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colegios_RegimenesIva_RegimenIvaId",
                table: "Colegios");

            migrationBuilder.DropTable(
                name: "RegimenesIva");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Colegios_Cedula",
                table: "Colegios");

            migrationBuilder.DropIndex(
                name: "IX_Colegios_Nit",
                table: "Colegios");

            migrationBuilder.DropIndex(
                name: "IX_Colegios_RegimenIvaId",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "Ciudad",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "CodigoActividadEconomica",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "Identificacion",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "IvaRetencion",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "ManejaAiu",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "RegimenIvaId",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "Responsabilidadesfiscales",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "TarifaIca",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "Tributos",
                table: "Colegios");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Nit",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Cedula",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
