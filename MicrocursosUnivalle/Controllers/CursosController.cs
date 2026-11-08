using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrocursosUnivalle.Data;
using MicrocursosUnivalle.Models;

namespace MicrocursosUnivalle.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _contexto;

        public CursosController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<IActionResult> Index()
        {
            var cursos = await _contexto.Cursos
                .Include(c => c.Lecciones)
                .Include(c => c.Inscripciones)
                .OrderByDescending(c => c.FechaCreacion)
                .ToListAsync();
            return View(cursos);
        }

        public async Task<IActionResult> Detalles(int id)
        {
            var curso = await _contexto.Cursos
                .Include(c => c.Lecciones.OrderBy(l => l.Orden))
                .Include(c => c.Inscripciones)
                    .ThenInclude(i => i.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null) return NotFound();
            return View(curso);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Curso curso)
        {
            if (ModelState.IsValid)
            {
                curso.FechaCreacion = DateTime.Now;
                _contexto.Cursos.Add(curso);
                await _contexto.SaveChangesAsync();
                TempData["Exito"] = "Curso creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var curso = await _contexto.Cursos.FindAsync(id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Curso curso)
        {
            if (id != curso.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _contexto.Update(curso);
                    await _contexto.SaveChangesAsync();
                    TempData["Exito"] = "Curso actualizado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_contexto.Cursos.Any(c => c.Id == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var curso = await _contexto.Cursos
                .Include(c => c.Lecciones)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var curso = await _contexto.Cursos.FindAsync(id);
            if (curso != null)
            {
                _contexto.Cursos.Remove(curso);
                await _contexto.SaveChangesAsync();
                TempData["Exito"] = "Curso eliminado correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPublicacion(int id)
        {
            var curso = await _contexto.Cursos.FindAsync(id);
            if (curso == null) return NotFound();

            curso.Publicado = !curso.Publicado;
            await _contexto.SaveChangesAsync();

            string estado = curso.Publicado ? "publicado" : "despublicado";
            TempData["Exito"] = $"Curso {estado} correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Suscriptores(int id)
        {
            var curso = await _contexto.Cursos
                .Include(c => c.Inscripciones)
                    .ThenInclude(i => i.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null) return NotFound();
            return View(curso);
        }
    }
}
