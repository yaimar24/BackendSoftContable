using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSoftContable.Migrations
{
    /// <inheritdoc />
    public partial class ReprTiid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colegios_TipoIdentificacion_TipoIdentificacionId",
                table: "Colegios");

            migrationBuilder.DropIndex(
                name: "IX_Colegios_TipoIdentificacionId",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "ArchivoDianPath",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "TipoIdentificacionId",
                table: "Colegios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArchivoDianPath",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoIdentificacionId",
                table: "Colegios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colegios_TipoIdentificacionId",
                table: "Colegios",
                column: "TipoIdentificacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Colegios_TipoIdentificacion_TipoIdentificacionId",
                table: "Colegios",
                column: "TipoIdentificacionId",
                principalTable: "TipoIdentificacion",
                principalColumn: "Id");
        }
    }
}
