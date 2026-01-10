using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSoftContable.Migrations
{
    /// <inheritdoc />
    public partial class AddCiudad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ciudad",
                table: "Colegios");

            // 🔹 CiudadId ahora es nullable
            migrationBuilder.AddColumn<int>(
                name: "CiudadId",
                table: "Colegios",
                type: "int",
                nullable: true); // antes era false

            // 🔹 Crear tabla Ciudad
            migrationBuilder.CreateTable(
                name: "Ciudad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: true) // nuevo campo
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ciudad", x => x.Id);
                });

            // 🔹 Insert de ciudades
            migrationBuilder.Sql(@"
                INSERT INTO Ciudad (Nombre, Codigo) VALUES
                ('Bogotá', 'BOG'),
                ('Medellín', 'MED'),
                ('Cali', 'CAL'),
                ('Barranquilla', 'BAR'),
                ('Cartagena', 'CAR'),
                ('Soledad', 'SOL'),
                ('Malambo', 'MAL'),
                ('Sabanalarga', 'SAB'),
                ('Puerto Colombia', 'PCO'),
                ('Galapa', 'GAL'),
                ('Baranoa', 'BARA'),
                ('Santa Marta', 'STM'),
                ('Bucaramanga', 'BCG'),
                ('Cúcuta', 'CUC'),
                ('Pereira', 'PER'),
                ('Ibagué', 'IBG'),
                ('Villavicencio', 'VMC'),
                ('Sincelejo', 'SIN'),
                ('Valledupar', 'VAL'),
                ('Neiva', 'NEI');
            ");

            // 🔹 Crear índice
            migrationBuilder.CreateIndex(
                name: "IX_Colegios_CiudadId",
                table: "Colegios",
                column: "CiudadId");

            // 🔹 Agregar FK
            migrationBuilder.AddForeignKey(
                name: "FK_Colegios_Ciudad_CiudadId",
                table: "Colegios",
                column: "CiudadId",
                principalTable: "Ciudad",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict); // Restrict para nullable
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colegios_Ciudad_CiudadId",
                table: "Colegios");

            migrationBuilder.DropTable(
                name: "Ciudad");

            migrationBuilder.DropIndex(
                name: "IX_Colegios_CiudadId",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "CiudadId",
                table: "Colegios");

            migrationBuilder.AddColumn<string>(
                name: "Ciudad",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
