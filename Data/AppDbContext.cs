
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pagina_proyecto.Models;
using Pagina_proyecto.Models.Entities;

namespace Pagina_proyecto.Areas.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // -------------------------
        // EXISTENTES
        // -------------------------
        public DbSet<CargaNoticia> Noticias { get; set; }
        public DbSet<ImagenesEnNoticias> Imagenes { get; set; }
        public DbSet<CargaCalendario> Calendario { get; set; }
        public DbSet<MateriasDrive> MateriasDrive { get; set; }
        public DbSet<OfertaCalificada> RecomendacionesFCE { get; set; }

        // -------------------------
        // ACADÉMICO
        // -------------------------
        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<Materia> Materias { get; set; }

        /* 🔹 NUEVA TABLA INTERMEDIA */
        public DbSet<CarrerasMateria> CarrerasMateria { get; set; }

        public DbSet<Correlativa> Correlativas { get; set; }
        public DbSet<UsuarioCarrera> UsuarioCarreras { get; set; }
        public DbSet<MateriaAprobadaAlumno> MateriasAprobadas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------
            // TABLAS
            // -------------------------
            modelBuilder.Entity<Carrera>().ToTable("Carreras");
            modelBuilder.Entity<Materia>().ToTable("Materias");
            modelBuilder.Entity<CarrerasMateria>().ToTable("CarrerasMateria");
            modelBuilder.Entity<Correlativa>().ToTable("Correlativas");
            modelBuilder.Entity<UsuarioCarrera>().ToTable("UsuarioCarreras");
            modelBuilder.Entity<MateriaAprobadaAlumno>().ToTable("MateriasAprobadasAlumno");

            // -------------------------
            // CLAVES PRIMARIAS
            // -------------------------
            modelBuilder.Entity<Carrera>()
                .HasKey(c => c.IdCarrera);

            modelBuilder.Entity<Materia>()
                .HasKey(m => m.IdMateria);

            modelBuilder.Entity<CarrerasMateria>()
                .HasKey(cm => new { cm.IdCarrera, cm.IdMateria });

            modelBuilder.Entity<Correlativa>()
                .HasKey(c => new { c.IdMateria, c.IdMateriaCorrelativa });

            modelBuilder.Entity<UsuarioCarrera>()
                .HasKey(uc => new { uc.IdUsuario, uc.IdCarrera });

            modelBuilder.Entity<MateriaAprobadaAlumno>()
                .HasKey(ma => new { ma.IdUsuario, ma.IdCarrera, ma.IdMateria });

            // -------------------------
            // COLUMNAS EXPLÍCITAS
            // -------------------------
            modelBuilder.Entity<Materia>()
                .Property(m => m.Nombre)
                .HasColumnName("NombreMateria");

            // -------------------------
            // RELACIÓN N-N
            // Carrera ↔ Materia
            // -------------------------
            modelBuilder.Entity<CarrerasMateria>()
                .HasOne(cm => cm.Carrera)
                .WithMany(c => c.CarrerasMateria)
                .HasForeignKey(cm => cm.IdCarrera)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CarrerasMateria>()
                .HasOne(cm => cm.Materia)
                .WithMany(m => m.CarrerasMateria)
                .HasForeignKey(cm => cm.IdMateria)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------
            // CORRELATIVAS
            // -------------------------
            modelBuilder.Entity<Correlativa>()
                .HasOne(c => c.Materia)
                .WithMany(m => m.Correlativas)
                .HasForeignKey(c => c.IdMateria)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Correlativa>()
                .HasOne(c => c.MateriaCorrelativa)
                .WithMany(m => m.EsCorrelativaDe)
                .HasForeignKey(c => c.IdMateriaCorrelativa)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // USUARIO ↔ CARRERA
            // -------------------------
            modelBuilder.Entity<UsuarioCarrera>()
                .HasOne(uc => uc.Carrera)
                .WithMany(c => c.Usuarios)
                .HasForeignKey(uc => uc.IdCarrera);

            modelBuilder.Entity<UsuarioCarrera>()
                .HasOne(uc => uc.Usuario)
                .WithMany(u => u.UsuarioCarreras)
                .HasForeignKey(uc => uc.IdUsuario);

            // -------------------------
            // APROBADAS
            // -------------------------
            modelBuilder.Entity<MateriaAprobadaAlumno>()
                .HasOne(ma => ma.Usuario)
                .WithMany()
                .HasForeignKey(ma => ma.IdUsuario);

            modelBuilder.Entity<MateriaAprobadaAlumno>()
                .HasOne(ma => ma.Carrera)
                .WithMany()
                .HasForeignKey(ma => ma.IdCarrera);

            modelBuilder.Entity<MateriaAprobadaAlumno>()
                .HasOne(ma => ma.Materia)
                .WithMany()
                .HasForeignKey(ma => ma.IdMateria);

            // -------------------------
            // Noticias / Imágenes
            // -------------------------
            modelBuilder.Entity<ImagenesEnNoticias>()
                .HasOne(i => i.CargaNoticia)
                .WithMany(n => n.Imagenes)
                .HasForeignKey(i => i.NoticiaId);

            modelBuilder.Entity<OfertaCalificada>()
                .ToTable("RecomendacionesFCE");
        }
    }
}

