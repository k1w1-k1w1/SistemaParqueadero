using Microsoft.EntityFrameworkCore;
using SistemaParqueadero.Models;

namespace SistemaParqueadero.API.Data
{
    public class ParqueaderoDbContext : DbContext
    {
        public ParqueaderoDbContext(DbContextOptions<ParqueaderoDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Usuario1> Usuarios1 { get; set; }
        public DbSet<RegistroParqueo> RegistrosParqueo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("parqueadero");
            // Configuración de Vehiculo
            modelBuilder.Entity<Vehiculo>(entity =>
            {
                entity.HasIndex(v => v.Placa).IsUnique();
                entity.Property(v => v.Placa).IsRequired().HasMaxLength(10);
            });

            // Configuración de Usuario
            modelBuilder.Entity<Usuario1>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Rol).IsRequired().HasMaxLength(20);
            });

            // Configuración de RegistroParqueo
            modelBuilder.Entity<RegistroParqueo>(entity =>
            {
                entity.Property(r => r.TipoTarifa)
                    .HasConversion<string>() // Guarda el enum como string
                    .HasMaxLength(20);

                entity.Property(r => r.Estado).IsRequired().HasMaxLength(20);

                // Relación con Vehiculo
                entity.HasOne(r => r.Vehiculo)
                    .WithMany(v => v.Registros)
                    .HasForeignKey(r => r.VehiculoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Usuario de Ingreso
                entity.HasOne(r => r.UsuarioIngreso)
                  .WithMany(u => u.RegistrosIngreso)
                  .HasForeignKey(r => r.UsuarioIngresoId)
                  .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.UsuarioSalida)
                  .WithMany(u => u.RegistrosSalida)
                  .HasForeignKey(r => r.UsuarioSalidaId)
                  .OnDelete(DeleteBehavior.Restrict);

                // Índices para búsquedas rápidas
                entity.HasIndex(r => r.Estado);
                entity.HasIndex(r => r.FechaHoraIngreso);
                entity.HasIndex(r => r.FechaHoraSalida);
            });

            // Datos semilla (usuarios iniciales)
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Usuario Administrador
            modelBuilder.Entity<Usuario1>().HasData(
                new Usuario1
                {
                    UsuarioId = 1,
                    NombreCompleto = "Administrador del Sistema",
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Rol = "Administrador",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                },
                new Usuario1
                {
                    UsuarioId = 2,
                    NombreCompleto = "Operador Principal",
                    Username = "operador",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Oper123!"),
                    Rol = "Operador",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                }
            );
        }
    }
}