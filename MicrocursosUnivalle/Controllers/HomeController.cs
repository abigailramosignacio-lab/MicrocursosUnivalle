using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrocursosUnivalle.Data;
using MicrocursosUnivalle.Models;
using System.Diagnostics;

namespace MicrocursosUnivalle.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _contexto;

        public HomeController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        // Página principal: catálogo de cursos publicados
        public async Task<IActionResult> Index()
        {
            var cursos = await _contexto.Cursos
                .Where(c => c.Publicado)
                .OrderByDescending(c => c.FechaCreacion)
                .ToListAsync();
            return View(cursos);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
