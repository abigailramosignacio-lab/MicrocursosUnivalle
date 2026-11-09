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

        // Ver curso con lecciones (solo si está inscrito)
        public async Task<IActionResult> VerCurso(int id)
        {
            var idUsuario = _gestorUsuarios.GetUserId(User)!;

            var curso = await _contexto.Cursos
                .Include(c => c.Lecciones.OrderBy(l => l.Orden))
                .FirstOrDefaultAsync(c => c.Id == id && c.Publicado);

            if (curso == null) return NotFound();

            // Verificación en servidor: el usuario debe estar inscrito
            var inscripcion = await _contexto.Inscripciones
                .FirstOrDefaultAsync(i => i.UsuarioId == idUsuario && i.CursoId == id);

            if (inscripcion == null)
            {
                TempData["Error"] = "Debes inscribirte al curso para ver las lecciones.";
                return RedirectToAction("Detalle", "Home", new { id });
            }

            var idsCompletadas = await _contexto.LeccionesCompletadas
                .Where(lc => lc.InscripcionId == inscripcion.Id)
                .Select(lc => lc.LeccionId)
                .ToListAsync();

            var modelo = new DetalleCursoVM
            {
                Curso = curso,
                EstaInscrito = true,
                InscripcionId = inscripcion.Id,
                Lecciones = curso.Lecciones.OrderBy(l => l.Orden).ToList(),
                LeccionesCompletadasIds = idsCompletadas
            };

            return View(modelo);
        }

        // Ver contenido de una lección (solo si está inscrito)
        public async Task<IActionResult> VerLeccion(int idLeccion)
        {
            var idUsuario = _gestorUsuarios.GetUserId(User)!;

            var leccion = await _contexto.Lecciones
                .Include(l => l.Curso)
                .FirstOrDefaultAsync(l => l.Id == idLeccion);

            if (leccion == null) return NotFound();

            // Fix: validar que el curso esté publicado
            if (leccion.Curso == null || !leccion.Curso.Publicado)
            {
                TempData["Error"] = "Este curso ya no está disponible.";
                return RedirectToAction("MisCursos");
            }

            // Verificación en servidor
            var inscripcion = await _contexto.Inscripciones
                .FirstOrDefaultAsync(i => i.UsuarioId == idUsuario && i.CursoId == leccion.CursoId);

            if (inscripcion == null)
            {
                TempData["Error"] = "No tienes acceso a esta lección.";
                return RedirectToAction("Index", "Home");
            }

            var yaCompletada = await _contexto.LeccionesCompletadas
                .AnyAsync(lc => lc.InscripcionId == inscripcion.Id && lc.LeccionId == idLeccion);

            ViewBag.Inscripcion = inscripcion;
            ViewBag.YaCompletada = yaCompletada;
            return View(leccion);
        }

        // POST: Marcar lección como completada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarCompletada(int idInscripcion, int idLeccion)
        {
            var idUsuario = _gestorUsuarios.GetUserId(User)!;

            // Verificación en servidor: la inscripción debe pertenecer al usuario
            var inscripcion = await _contexto.Inscripciones
                .FirstOrDefaultAsync(i => i.Id == idInscripcion && i.UsuarioId == idUsuario);

            if (inscripcion == null) return Forbid();

            bool yaExiste = await _contexto.LeccionesCompletadas
                .AnyAsync(lc => lc.InscripcionId == idInscripcion && lc.LeccionId == idLeccion);

            if (!yaExiste)
            {
                var completada = new LeccionCompletada
                {
                    InscripcionId = idInscripcion,
                    LeccionId = idLeccion,
                    FechaCompletado = DateTime.Now
                };
                _contexto.LeccionesCompletadas.Add(completada);
                await _contexto.SaveChangesAsync();
            }

            return RedirectToAction("VerCurso", new { id = inscripcion.CursoId });
        }

        // Ver progreso de un curso
        public async Task<IActionResult> MiProgreso(int idCurso)
        {
            var idUsuario = _gestorUsuarios.GetUserId(User)!;

            var inscripcion = await _contexto.Inscripciones
                .Include(i => i.LeccionesCompletadas)
                .FirstOrDefaultAsync(i => i.UsuarioId == idUsuario && i.CursoId == idCurso);

            if (inscripcion == null) return RedirectToAction("MisCursos");

            var curso = await _contexto.Cursos
                .Include(c => c.Lecciones)
                .FirstOrDefaultAsync(c => c.Id == idCurso);

            if (curso == null) return NotFound();

            var modelo = new ProgresoVM
            {
                Curso = curso,
                TotalLecciones = curso.Lecciones.Count,
                LeccionesCompletadas = inscripcion.LeccionesCompletadas.Count
            };

            return View(modelo);
        }
    }
}
