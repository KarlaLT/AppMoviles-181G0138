using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace AplicacionU1_API.Models
{
    public partial class itesrcne_181g0138Context : DbContext
    {
        public itesrcne_181g0138Context()
        {
        }

        public itesrcne_181g0138Context(DbContextOptions<itesrcne_181g0138Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Cupones> Cupones { get; set; }
        public virtual DbSet<Recomendaciones> Recomendaciones { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
               // optionsBuilder.UseMySql("server=204.93.216.11;user=itesrcne_karla;password=181G0138;database=itesrcne_181g0138", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.3.29-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8");

            modelBuilder.Entity<Cupones>(entity =>
            {
                entity.HasKey(e => e.IdCupon)
                    .HasName("PRIMARY");

                entity.ToTable("cupones");

                entity.Property(e => e.IdCupon)
                    .HasColumnType("int(11)")
                    .HasColumnName("idCupon");

                entity.Property(e => e.Categoria)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.Descripcion).IsRequired();

                entity.Property(e => e.FechaFin).HasColumnType("date");

                entity.Property(e => e.FechaInicio).HasColumnType("date");

                entity.Property(e => e.Tienda)
                    .IsRequired()
                    .HasMaxLength(60);

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(60);
            });

            modelBuilder.Entity<Recomendaciones>(entity =>
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
