using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSoftContable.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCamposResDian : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaCertificado",
                table: "Colegios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCertificado",
                table: "Colegios",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
