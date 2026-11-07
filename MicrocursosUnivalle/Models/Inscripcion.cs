using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MicrocursosUnivalle.Models
{
    public class Inscripcion
    {
        public int Id { get; set; }

        [Display(Name = "Fecha de inscripción")]
        public DateTime FechaInscripcion { get; set; } = DateTime.Now;

        // FK Usuario
        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        [ForeignKey("UsuarioId")]
        public IdentityUser? Usuario { get; set; }

        // FK Curso
        [Required]
        public int CursoId { get; set; }

        [ForeignKey("CursoId")]
        public Curso? Curso { get; set; }

        // Navegación
        public ICollection<LeccionCompletada> LeccionesCompletadas { get; set; } = new List<LeccionCompletada>();
    }
}
