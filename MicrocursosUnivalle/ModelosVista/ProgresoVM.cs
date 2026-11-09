using MicrocursosUnivalle.Models;

namespace MicrocursosUnivalle.ModelosVista
{
    public class ProgresoVM
    {
        public Curso Curso { get; set; } = null!;
        public int TotalLecciones { get; set; }
        public int LeccionesCompletadas { get; set; }
        public double PorcentajeProgreso => TotalLecciones == 0 ? 0 : Math.Round((double)LeccionesCompletadas / TotalLecciones * 100, 1);
    }
}
