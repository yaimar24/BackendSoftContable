using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSoftContable.Migrations
{
    /// <inheritdoc />
    public partial class impsdobleValorem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UsaDobleImpuesto",
                table: "Colegios",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "usaImpuestoAdValorem",
                table: "Colegios",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsaDobleImpuesto",
                table: "Colegios");

            migrationBuilder.DropColumn(
                name: "usaImpuestoAdValorem",
                table: "Colegios");
        }
    }
}
