using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSoftContable.Migrations
{
    public partial class ActividadEconomica : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Crear la tabla maestra primero
            migrationBuilder.CreateTable(
                name: "ActividadEconomica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActividadEconomica", x => x.Id);
                });

            // 2. INSERTAR DATOS REALES DE COLOMBIA (CIIU Rev. 4)
            // Esto asegura que existan IDs para que los colegios puedan referenciarlos
            migrationBuilder.InsertData(
                table: "ActividadEconomica",
                columns: new[] { "Codigo", "Descripcion" },
                values: new object[,]
                {
                    { "8511", "Educación de instituciones de educación primera infancia" },
                    { "8512", "Educación de instituciones de educación básica primaria" },
                    { "8513", "Educación de instituciones de educación básica secundaria" },
                    { "8521", "Educación de instituciones de educación media académica" },
                    { "8522", "Educación de instituciones de educación media técnica y de formación laboral" },
                    { "8530", "Establecimientos que prestan el servicio de educación superior" },
                    { "8544", "Enseñanza deportiva y recreativa" },
                    { "8551", "Formación académica no formal" }
                });

            // 3. Eliminar columna vieja de texto
            migrationBuilder.DropColumn(
                name: "CodigoActividadEconomica",
                table: "Colegios");

            // 4. Agregar la nueva columna FK
            // La ponemos como nullable: true temporalmente si ya tienes datos en Colegios
            // O dejamos defaultValue: 1 para que apunte al primer registro insertado
            migrationBuilder.AddColumn<int>(
                name: "ActividadEconomicaId",
                table: "Colegios",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Colegios_ActividadEconomicaId",
                table: "Colegios",
                column: "ActividadEconomicaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Colegios_ActividadEconomica_ActividadEconomicaId",
                table: "Colegios",
                column: "ActividadEconomicaId",
                principalTable: "ActividadEconomica",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colegios_ActividadEconomica_ActividadEconomicaId",
                table: "Colegios");

            migrationBuilder.DropTable(
                name: "ActividadEconomica");

            migrationBuilder.DropIndex(
                name: "IX_Colegios_ActividadEconomicaId",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "ActividadEconomicaId",
                table: "Colegios");

            migrationBuilder.AddColumn<string>(
                name: "CodigoActividadEconomica",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}