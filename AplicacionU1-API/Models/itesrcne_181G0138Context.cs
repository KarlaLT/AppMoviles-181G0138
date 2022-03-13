using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace AplicacionU1_API.Models
{
    public partial class itesrcne_181G0138Context : DbContext
    {
        public itesrcne_181G0138Context()
        {
        }

        public itesrcne_181G0138Context(DbContextOptions<itesrcne_181G0138Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Recomendacione> Recomendaciones { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8");

            modelBuilder.Entity<Recomendacione>(entity =>
            {
                entity.ToTable("recomendaciones");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Autor)
                    .IsRequired()
                    .HasMaxLength(60);

                entity.Property(e => e.Eliminado)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Genero)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.Opinion).IsRequired();

                entity.Property(e => e.Puntuacion).HasColumnType("int(11)");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.Property(e => e.TituloLibro)
                    .IsRequired()
                    .HasMaxLength(60);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
