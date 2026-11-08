using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrocursosUnivalle.Data;
using MicrocursosUnivalle.Models;

namespace MicrocursosUnivalle.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class LeccionesController : Controller
    {
        private readonly ApplicationDbContext _contexto;

        public LeccionesController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<IActionResult> Index(int cursoId)
        {
            var curso = await _contexto.Cursos
                .Include(c => c.Lecciones.OrderBy(l => l.Orden))
                .FirstOrDefaultAsync(c => c.Id == cursoId);

            if (curso == null) return NotFound();

            ViewBag.Curso = curso;
            return View(curso.Lecciones.OrderBy(l => l.Orden).ToList());
        }

        public async Task<IActionResult> Crear(int cursoId)
        {
            var curso = await _contexto.Cursos.FindAsync(cursoId);
            if (curso == null) return NotFound();

            var leccion = new Leccion { CursoId = cursoId };
            ViewBag.Curso = curso;
            return View(leccion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Leccion leccion)
        {
            if (ModelState.IsValid)
            {
                _contexto.Lecciones.Add(leccion);
                await _contexto.SaveChangesAsync();
                TempData["Exito"] = "Lección creada correctamente.";
                return RedirectToAction(nameof(Index), new { cursoId = leccion.CursoId });
            }
            ViewBag.Curso = await _contexto.Cursos.FindAsync(leccion.CursoId);
            return View(leccion);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var leccion = await _contexto.Lecciones
                .Include(l => l.Curso)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leccion == null) return NotFound();
            ViewBag.Curso = leccion.Curso;
            return View(leccion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Leccion leccion)
        {
            if (id != leccion.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _contexto.Update(leccion);
                await _contexto.SaveChangesAsync();
                TempData["Exito"] = "Lección actualizada correctamente.";
                return RedirectToAction(nameof(Index), new { cursoId = leccion.CursoId });
            }
            ViewBag.Curso = await _contexto.Cursos.FindAsync(leccion.CursoId);
            return View(leccion);
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var leccion = await _contexto.Lecciones
                .Include(l => l.Curso)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leccion == null) return NotFound();
            return View(leccion);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var leccion = await _contexto.Lecciones.FindAsync(id);
            if (leccion != null)
            {
                int idCurso = leccion.CursoId;
                _contexto.Lecciones.Remove(leccion);
                await _contexto.SaveChangesAsync();
                TempData["Exito"] = "Lección eliminada correctamente.";
                return RedirectToAction(nameof(Index), new { cursoId = idCurso });
            }
            return RedirectToAction("Index", "Cursos");
        }
    }
}
