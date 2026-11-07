using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Collections.Specialized.BitVector32;

namespace MicrocursosUnivalle.Models
{
    public class LeccionCompletada
    {
        public int Id { get; set; }

        [Display(Name = "Fecha de completado")]
        public DateTime FechaCompletado { get; set; } = DateTime.Now;

        // FK Inscripcion
        [Required]
        public int InscripcionId { get; set; }

        [ForeignKey("InscripcionId")]
        public Inscripcion? Inscripcion { get; set; }

        // FK Leccion
        [Required]
        public int LeccionId { get; set; }

        [ForeignKey("LeccionId")]
        public Leccion? Leccion { get; set; }
    }
}
