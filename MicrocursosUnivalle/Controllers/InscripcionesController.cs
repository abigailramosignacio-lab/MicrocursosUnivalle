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

        // POST: Inscribirse en un curso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inscribir(int idCurso)
        {
            var idUsuario = _gestorUsuarios.GetUserId(User)!;

            var curso = await _contexto.Cursos.FindAsync(idCurso);
            if (curso == null || !curso.Publicado)
            {
                TempData["Error"] = "El curso no está disponible.";
                return RedirectToAction("Index", "Home");
            }

            // Verificar si ya está inscrito (seguridad en servidor)
            bool yaInscrito = await _contexto.Inscripciones
                .AnyAsync(i => i.UsuarioId == idUsuario && i.CursoId == idCurso);

            if (yaInscrito)
            {
                TempData["Error"] = "Ya estás inscrito en este curso.";
                return RedirectToAction("VerCurso", new { id = idCurso });
            }

            var nuevaInscripcion = new Inscripcion
            {
                UsuarioId = idUsuario,
                CursoId = idCurso,
                FechaInscripcion = DateTime.Now
            };

            _contexto.Inscripciones.Add(nuevaInscripcion);
            await _contexto.SaveChangesAsync();
            TempData["Exito"] = "¡Te has inscrito correctamente al curso!";
            return RedirectToAction("VerCurso", new { id = idCurso });
        }
    }
}
