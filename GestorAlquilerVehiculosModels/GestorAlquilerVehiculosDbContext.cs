using GestorAlquilerVehiculos.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorAlquilerVehiculos.Data
{
    public class GestorAlquilerVehiculosDbContext : DbContext
    {
        public GestorAlquilerVehiculosDbContext(DbContextOptions<GestorAlquilerVehiculosDbContext> options)
               : base(options)
        { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ClienteReserva> ClientesReserva { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Mantenimiento> Mantenimientos { get; set; }
        public DbSet<Reporte> Reportes { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<EntregaDevolucion> EntregasDevoluciones { get; set; }
        public DbSet<CargoAdicional> CargosAdicionales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configuración de precisión para DECIMALs ---
            modelBuilder.Entity<Reserva>()
                .Property(r => r.CostoTotal)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Vehiculo>()
                .Property(v => v.PrecioPorDia)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Mantenimiento>()
                .Property(m => m.Costo)
                .HasPrecision(10, 2);

            modelBuilder.Entity<EntregaDevolucion>()
                .Property(e => e.CargosAdicionales)
                .HasPrecision(10, 2);

            modelBuilder.Entity<CargoAdicional>()
                .Property(c => c.Monto)
                .HasPrecision(10, 2);

            // ✅ Relaciones opcionales con constraint
            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.HasOne(r => r.Usuario)
                    .WithMany(u => u.Reservas)
                    .HasForeignKey(r => r.UsuarioID)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.ClienteReserva)
                    .WithMany(c => c.Reservas)
                    .HasForeignKey(r => r.ClienteReservaID)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Reserva_ClienteOUsuario",
                        "(UsuarioID IS NOT NULL AND ClienteReservaID IS NULL) OR " +
                        "(UsuarioID IS NULL AND ClienteReservaID IS NOT NULL)");
                });
            });
        }



    }
}
