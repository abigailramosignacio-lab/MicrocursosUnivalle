using MicrocursosUnivalle.Models;

namespace MicrocursosUnivalle.ModelosVista
{
    public class DetalleCursoVM
    {
        public Curso Curso { get; set; } = null!;
        public bool EstaInscrito { get; set; }
        public int? InscripcionId { get; set; }
        public List<Leccion> Lecciones { get; set; } = new();
        public List<int> LeccionesCompletadasIds { get; set; } = new();
    }
}
