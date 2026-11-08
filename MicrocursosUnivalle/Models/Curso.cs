using System.ComponentModel.DataAnnotations;
using static System.Collections.Specialized.BitVector32;

namespace MicrocursosUnivalle.Models
{
    public class Curso
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(200, ErrorMessage = "El título no puede superar 200 caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nivel es obligatorio.")]
        [Display(Name = "Nivel")]
        public string Nivel { get; set; } = string.Empty; 

        [Required(ErrorMessage = "La duración es obligatoria.")]
        [Display(Name = "Duración (horas)")]
        [Range(1, 1000, ErrorMessage = "La duración debe ser mayor a 0.")]
        public int DuracionHoras { get; set; }

        [Display(Name = "Imagen de portada (URL)")]
        public string? ImagenUrl { get; set; }

        [Display(Name = "Publicado")]
        public bool Publicado { get; set; } = false;

        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public ICollection<Leccion> Lecciones { get; set; } = new List<Leccion>();
        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
    }
}
