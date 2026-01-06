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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación Colegio (1) -> Usuarios (muchos)
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Colegio)
                .WithMany(c => c.Usuarios)
                .HasForeignKey(u => u.ColegioId)
                .OnDelete(DeleteBehavior.Cascade); 

            // Campos obligatorios
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Email)
                .IsRequired();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.PasswordHash)
                .IsRequired();
        }
    }
}
