using Dsw2026Ej15.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dsw2026Ej15.Data
{
    public class Dsw2026Ej15DbContext : DbContext
    {
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Speciality> Specialities { get; set; }

        public Dsw2026Ej15DbContext(DbContextOptions<Dsw2026Ej15DbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo de Speciality
            modelBuilder.Entity<Speciality>(entity =>
            {
                entity.ToTable("Specialities");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Description).IsRequired().HasMaxLength(250);
            });

            // Mapeo de Doctor
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.ToTable("Doctors");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
                entity.Property(d => d.LicenseNumber).IsRequired().HasMaxLength(50);
                entity.Property(d => d.IsActive).IsRequired();

                // Relación uno a muchos unidireccional (un Doctor tiene una Especialidad)
                entity.HasOne(d => d.Speciality)
                      .WithMany()
                      .IsRequired();
            });
        }
    }
}
