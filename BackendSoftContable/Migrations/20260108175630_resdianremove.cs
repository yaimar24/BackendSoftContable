using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSoftContable.Migrations
{
    /// <inheritdoc />
    public partial class resdianremove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResDian",
                table: "Colegios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResDian",
                table: "Colegios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
