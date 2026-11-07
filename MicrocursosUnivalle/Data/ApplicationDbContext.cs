using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MicrocursosUnivalle.Models;

namespace MicrocursosUnivalle.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Leccion> Lecciones { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<LeccionCompletada> LeccionesCompletadas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Índice único: un usuario no puede inscribirse dos veces al mismo curso
            builder.Entity<Inscripcion>()
                .HasIndex(i => new { i.UsuarioId, i.CursoId })
                .IsUnique();

            // Índice único: una lección no se puede marcar completada dos veces en la misma inscripción
            builder.Entity<LeccionCompletada>()
                .HasIndex(lc => new { lc.InscripcionId, lc.LeccionId })
                .IsUnique();

            // Configurar cascada para evitar múltiples rutas de cascada
            builder.Entity<LeccionCompletada>()
                .HasOne(lc => lc.Leccion)
                .WithMany(l => l.LeccionesCompletadas)
                .HasForeignKey(lc => lc.LeccionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LeccionCompletada>()
                .HasOne(lc => lc.Inscripcion)
                .WithMany(i => i.LeccionesCompletadas)
                .HasForeignKey(lc => lc.InscripcionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Inscripcion>()
                .HasOne(i => i.Curso)
                .WithMany(c => c.Inscripciones)
                .HasForeignKey(i => i.CursoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Leccion>()
                .HasOne(l => l.Curso)
                .WithMany(c => c.Lecciones)
                .HasForeignKey(l => l.CursoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
