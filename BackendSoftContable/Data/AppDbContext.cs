using Microsoft.EntityFrameworkCore;
using BackendSoftContable.Models;
using BackendSoftContable.Models.Terceros;
using BackendSoftContable.Models.TipoFactura;

namespace BackendSoftContable.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Colegio> Colegios { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<RegimenIva> RegimenesIva { get; set; } = null!;
        public DbSet<ResponsabilidadFiscal> ResponsabilidadFiscal { get; set; } = null!;
        public DbSet<Tributo> Tributo { get; set; } = null!;
        public DbSet<Ciudad> Ciudad { get; set; } = null!;
        public DbSet<TipoIdentificacion> TipoIdentificacion { get; set; } = null!;
        public DbSet<ActividadEconomica> ActividadEconomica { get; set; } = null!;
        public DbSet<Roles> Roles { get; set; } = null!;
        public DbSet<RepresentanteLegal> RepresentantesLegales { get; set; } = null!;
        public DbSet<Puc> Puc { get; set; }

        public DbSet<TipoPersona> TipoPersona { get; set; } = null!;
        public DbSet<Tercero> Tercero { get; set; } = null!;
        public DbSet<TerceroCategoria> TerceroCategoria { get; set; } = null!;
        public DbSet<TerceroInformacionFiscal> TerceroInformacionFiscal { get; set; } = null!;
        public DbSet<TerceroResponsabilidad> TerceroResponsabilidad { get; set; } = null!;
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<AuditoriaLog> AuditoriaLog { get; set; } = null!;
        public DbSet<TipoFactura> TipoFacturas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- CONFIGURACIÓN DE GUIDS ---

            // Colegio Id como GUID
            modelBuilder.Entity<Colegio>()
                .Property(c => c.Id)
                .HasDefaultValueSql("NEWID()"); // SQL Server generará el GUID automáticamente

            // Usuario Id como GUID
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Id)
                .HasDefaultValueSql("NEWID()");

            // --- RELACIONES ---

            // Colegio -> Usuarios (1:N)
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Colegio)
                .WithMany(c => c.Usuarios)
                .HasForeignKey(u => u.ColegioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Roles -> Usuarios (1:N)
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Roles)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolesId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- RESTRICCIONES Y ÚNICOS ---

            modelBuilder.Entity<Colegio>()
                .HasIndex(c => c.Nit)
                .IsUnique();

            modelBuilder.Entity<Colegio>()
                .Property(c => c.Nit)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Email)
                .HasMaxLength(150)
                .IsRequired();

            // RepresentanteLegal -> TipoIdentificacion
            modelBuilder.Entity<RepresentanteLegal>()
                .HasOne(r => r.TipoIdentificacion)
                .WithMany()
                .HasForeignKey(r => r.TipoIdentificacionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RepresentanteLegal>()
                .HasIndex(r => r.NumeroIdentificacion)
                .IsUnique();

            modelBuilder.Entity<RepresentanteLegal>()
                .Property(r => r.NumeroIdentificacion)
                .HasMaxLength(20)
                .IsRequired();

            // RepresentanteLegal -> Colegio (1:N)
            modelBuilder.Entity<RepresentanteLegal>()
                .HasOne(r => r.Colegio)
                .WithMany(c => c.RepresentantesLegales)
                .HasForeignKey(r => r.ColegioId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Puc>()
                .Property(p => p.Naturaleza)
                .IsFixedLength();


            // --- CONFIGURACIÓN DE TERCEROS ---

            // Tercero Id como GUID
            modelBuilder.Entity<Tercero>()
                .Property(t => t.Id)
                .HasDefaultValueSql("NEWID()");

            // Identificación única a nivel global
            modelBuilder.Entity<Tercero>()
                .HasIndex(t => t.Identificacion)
                .IsUnique();

            // Relación Tercero -> TipoPersona
            modelBuilder.Entity<Tercero>()
                .HasOne(t => t.TipoPersona)
                .WithMany()
                .HasForeignKey(t => t.TipoPersonaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Tercero -> TipoIdentificacion
            modelBuilder.Entity<Tercero>()
                .HasOne(t => t.TipoIdentificacion)
                .WithMany()
                .HasForeignKey(t => t.TipoIdentificacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- CONFIGURACIÓN DE VINCULACIÓN (CATEGORÍAS) ---

            modelBuilder.Entity<TerceroCategoria>(entity =>
            {
                // Clave única compuesta: Un tercero no puede tener la misma categoría dos veces en el mismo colegio
                entity.HasIndex(tc => new { tc.TerceroId, tc.ColegioId, tc.CategoriaId })
          .IsUnique();

                // Relación con Colegio
                entity.HasOne<Colegio>()
                      .WithMany()
                      .HasForeignKey(tc => tc.ColegioId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación con RegimenIva
                entity.HasOne(tc => tc.RegimenIva)
                      .WithMany()
                      .HasForeignKey(tc => tc.RegimenIvaId)
                      .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(tc => tc.Categoria)
                     .WithMany(c => c.TerceroCategorias)
                     .HasForeignKey(tc => tc.CategoriaId)
                     .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Tercero>()
        .HasOne(t => t.InformacionFiscal)
        .WithOne(ifis => ifis.Tercero)
        .HasForeignKey<TerceroInformacionFiscal>(ifis => ifis.TerceroId);

            // Llave compuesta TerceroResponsabilidad
            modelBuilder.Entity<TerceroResponsabilidad>()
                .HasKey(tr => new { tr.TerceroId, tr.ResponsabilidadFiscalId });

            //PUC 

            modelBuilder.Entity<Puc>(entity =>
            {
                entity.ToTable("Puc");

                // 1. Definir Llave Compuesta: Codigo + ColegioId
                // Esto permite que el Codigo '1105' exista para ColegioId NULL 
                // y también para un ColegioId específico.
                entity.HasKey(p => new { p.Codigo, p.ColegioId });

                entity.Property(p => p.Codigo).HasMaxLength(20);

                entity.Property(p => p.Naturaleza)
                    .IsFixedLength()
                    .HasMaxLength(1)
                    .IsRequired();

                // 2. Relación con Colegio (Muchos PUC pertenecen a un Colegio)
                entity.HasOne(p => p.Colegio)
                    .WithMany()
                    .HasForeignKey(p => p.ColegioId)
                    .OnDelete(DeleteBehavior.Cascade); // Si se borra el colegio, se borran sus cuentas personalizadas

                // 3. Relación Autorreferencial (Padre -> Hijos)
                // NOTA: Al ser llave compuesta, la relación debe apuntar a ambos campos
                entity.HasOne(p => p.Padre)
                    .WithMany(p => p.Hijos)
                    .HasForeignKey(p => new { p.CodigoPadre, p.ColegioId })
                    .OnDelete(DeleteBehavior.Restrict);
            });

        }




    }

}