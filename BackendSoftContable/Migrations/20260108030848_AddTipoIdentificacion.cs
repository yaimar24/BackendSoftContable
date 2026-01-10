using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSoftContable.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoIdentificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoIdentificacionId",
                table: "Colegios",
                type: "int",
                nullable: false,
                defaultValue: 1); // temporal, se puede actualizar después del insert

            migrationBuilder.CreateTable(
                name: "TipoIdentificacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoDian = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoIdentificacion", x => x.Id);
                });

            // 🔹 Insert de tipos de identificación
            migrationBuilder.Sql(@"
                INSERT INTO TipoIdentificacion (Nombre, CodigoDian) VALUES
                ('Cédula de Ciudadanía', 'CC'),
                ('Cédula de Extranjería', 'CE'),
                ('NIT', 'NIT'),
                ('Pasaporte', 'PA'),
                ('Tarjeta de Identidad', 'TI');
            ");

            // 🔹 Crear índice
            migrationBuilder.CreateIndex(
                name: "IX_Colegios_TipoIdentificacionId",
                table: "Colegios",
                column: "TipoIdentificacionId");

            // 🔹 Foreign key
            migrationBuilder.AddForeignKey(
                name: "FK_Colegios_TipoIdentificacion_TipoIdentificacionId",
                table: "Colegios",
                column: "TipoIdentificacionId",
                principalTable: "TipoIdentificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict); // mejor usar Restrict si algunos colegios no tienen tipo
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colegios_TipoIdentificacion_TipoIdentificacionId",
                table: "Colegios");

            migrationBuilder.DropTable(
                name: "TipoIdentificacion");

            migrationBuilder.DropIndex(
                name: "IX_Colegios_TipoIdentificacionId",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "TipoIdentificacionId",
                table: "Colegios");
        }
    }
}
