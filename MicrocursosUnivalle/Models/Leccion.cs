using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicrocursosUnivalle.Models
{
    public class Leccion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título de la lección es obligatorio.")]
        [StringLength(200)]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El contenido es obligatorio.")]
        [Display(Name = "Contenido")]
        public string Contenido { get; set; } = string.Empty;

        [Display(Name = "Orden")]
        [Range(1, 9999)]
        public int Orden { get; set; } = 1;

        [Display(Name = "Duración (minutos)")]
        [Range(1, 600)]
        public int DuracionMinutos { get; set; } = 10;

        // FK
        [Display(Name = "Curso")]
        public int CursoId { get; set; }

        [ForeignKey("CursoId")]
        public Curso? Curso { get; set; }

        // Navegación
        public ICollection<LeccionCompletada> LeccionesCompletadas { get; set; } = new List<LeccionCompletada>();
    }
}
