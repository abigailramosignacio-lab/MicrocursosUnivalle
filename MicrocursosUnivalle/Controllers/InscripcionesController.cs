using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrocursosUnivalle.Data;
using MicrocursosUnivalle.Models;
using MicrocursosUnivalle.ModelosVista;

namespace MicrocursosUnivalle.Controllers
{
    [Authorize(Roles = "Suscriptor")]
    public class InscripcionesController : Controller
    {
        private readonly ApplicationDbContext _contexto;
        private readonly UserManager<IdentityUser> _gestorUsuarios;

        public InscripcionesController(ApplicationDbContext contexto, UserManager<IdentityUser> gestorUsuarios)
        {
            _contexto = contexto;
            _gestorUsuarios = gestorUsuarios;
        }

        // Mis cursos inscritos
        public async Task<IActionResult> MisCursos()
        {
            var idUsuario = _gestorUsuarios.GetUserId(User)!;
            var inscripciones = await _contexto.Inscripciones
                .Include(i => i.Curso)
                    .ThenInclude(c => c!.Lecciones)
                .Where(i => i.UsuarioId == idUsuario)
                .OrderByDescending(i => i.FechaInscripcion)
                .ToListAsync();

            return View(inscripciones);
        }
    }
}
