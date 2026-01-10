using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSoftContable.Migrations
{
    public partial class Roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. CREAR TABLA ROLES (Solo si no existe)
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'[Roles]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [Roles] (
                        [Id] int NOT NULL IDENTITY,
                        [Nombre] nvarchar(max) NOT NULL,
                        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
                    );
                END
            ");

            // 2. INSERTAR ROL ADMIN (Solo si no existe)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Roles WHERE Nombre = 'Admin')
                BEGIN
                    INSERT INTO Roles (Nombre) VALUES ('Admin');
                END
            ");

            // 3. AGREGAR COLUMNA ROLID A USUARIOS (Solo si no existe)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM sys.columns 
                    WHERE object_id = OBJECT_ID(N'[Usuarios]') 
                    AND name = N'RolId'
                )
                BEGIN
                    ALTER TABLE [Usuarios] ADD [RolId] int NOT NULL DEFAULT 0;
                END
            ");

            // 4. ACTUALIZAR USUARIOS EXISTENTES
            migrationBuilder.Sql(@"
                DECLARE @AdminId INT;
                SELECT TOP 1 @AdminId = Id FROM Roles WHERE Nombre = 'Admin';

                IF @AdminId IS NOT NULL
                BEGIN
                    UPDATE Usuarios
                    SET RolId = @AdminId
                    WHERE RolId = 0 OR RolId IS NULL;
                END
            ");

            // 5. CREAR ÍNDICE (Solo si no existe)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuarios_RolId' AND object_id = OBJECT_ID('Usuarios'))
                BEGIN
                    CREATE INDEX [IX_Usuarios_RolId] ON [Usuarios] ([RolId]);
                END
            ");

            // 6. CREAR LLAVE FORÁNEA CON NO ACTION (Solución al error 1785)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Usuarios_Roles_RolId')
                BEGIN
                    ALTER TABLE [Usuarios] ADD CONSTRAINT [FK_Usuarios_Roles_RolId] 
                    FOREIGN KEY ([RolId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Para revertir los cambios en orden inverso
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Usuarios_Roles_RolId')
                    ALTER TABLE [Usuarios] DROP CONSTRAINT [FK_Usuarios_Roles_RolId];

                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuarios_RolId' AND object_id = OBJECT_ID('Usuarios'))
                    DROP INDEX [IX_Usuarios_RolId] ON [Usuarios];
            ");

            migrationBuilder.DropColumn(
                name: "RolId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}