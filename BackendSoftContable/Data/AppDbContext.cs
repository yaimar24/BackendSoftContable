using Microsoft.EntityFrameworkCore;
using BackendSoftContable.Models;

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
        }
    }
}