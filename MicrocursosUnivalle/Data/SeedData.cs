using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MicrocursosUnivalle.Models;

namespace MicrocursosUnivalle.Data
{
    public static class SeedData
    {
        public static async Task InicializarRolesYAdmin(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Administrador", "Suscriptor" };
            foreach (var rol in roles)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    await roleManager.CreateAsync(new IdentityRole(rol));
                }
            }

            // Crear usuario administrador por defecto
            string adminEmail = "admin@microcursosunivalle.edu.co";
            string adminPassword = "Admin@123456";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var resultado = await userManager.CreateAsync(adminUser, adminPassword);
                if (resultado.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                }
            }
        }

        // Datos de ejemplo: cursos y lecciones
        public static async Task InicializarDatosEjemplo(IServiceProvider serviceProvider)
        {
            var contexto = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Solo insertar si no hay cursos
            if (await contexto.Cursos.AnyAsync()) return;

            var cursos = new List<Curso>
            {
                new Curso
                {
                    Titulo = "Introducción a HTML5",
                    Descripcion = "Aprende los fundamentos del lenguaje de marcado que estructura toda la web moderna. Desde la etiqueta <!DOCTYPE> hasta formularios accesibles.",
                    Nivel = "Básico",
                    DuracionHoras = 5,
                    Publicado = true,
                    FechaCreacion = DateTime.Now.AddDays(-30)
                },
                new Curso
                {
                    Titulo = "CSS3 para Diseñadores",
                    Descripcion = "Domina el diseño web con selectores avanzados, Flexbox, CSS Grid y animaciones para crear interfaces modernas y responsivas.",
                    Nivel = "Intermedio",
                    DuracionHoras = 8,
                    Publicado = true,
                    FechaCreacion = DateTime.Now.AddDays(-20)
                },
                new Curso
                {
                    Titulo = "JavaScript Moderno (ES6+)",
                    Descripcion = "Programa con las características modernas de JavaScript: arrow functions, desestructuración, promesas, async/await y módulos.",
                    Nivel = "Avanzado",
                    DuracionHoras = 12,
                    Publicado = true,
                    FechaCreacion = DateTime.Now.AddDays(-10)
                },
                new Curso
                {
                    Titulo = "C# Avanzado (En preparación)",
                    Descripcion = "Curso en desarrollo, próximamente disponible.",
                    Nivel = "Avanzado",
                    DuracionHoras = 15,
                    Publicado = false,
                    FechaCreacion = DateTime.Now
                }
            };

            await contexto.Cursos.AddRangeAsync(cursos);
            await contexto.SaveChangesAsync();

            var lecciones = new List<Leccion>
            {
                // HTML5
                new Leccion
                {
                    Titulo = "Estructura básica de HTML",
                    Contenido = "<h2>Estructura básica de HTML</h2><p>HTML (HyperText Markup Language) es el lenguaje que estructura el contenido de las páginas web. Toda página comienza con <code>&lt;!DOCTYPE html&gt;</code> y contiene las etiquetas <code>&lt;html&gt;</code>, <code>&lt;head&gt;</code> y <code>&lt;body&gt;</code>.</p>",
                    Orden = 1,
                    DuracionMinutos = 15,
                    CursoId = cursos[0].Id
                },
                new Leccion
                {
                    Titulo = "Etiquetas semánticas",
                    Contenido = "<h2>Etiquetas semánticas</h2><p>HTML5 introdujo etiquetas semánticas: <code>&lt;header&gt;</code>, <code>&lt;nav&gt;</code>, <code>&lt;main&gt;</code>, <code>&lt;article&gt;</code>, <code>&lt;section&gt;</code>, <code>&lt;aside&gt;</code> y <code>&lt;footer&gt;</code>. Mejoran la accesibilidad y el SEO.</p>",
                    Orden = 2,
                    DuracionMinutos = 20,
                    CursoId = cursos[0].Id
                },
                new Leccion
                {
                    Titulo = "Formularios en HTML5",
                    Contenido = "<h2>Formularios en HTML5</h2><p>Los formularios capturan datos del usuario. HTML5 agrega tipos de input: <code>email</code>, <code>tel</code>, <code>date</code>, <code>number</code>, y atributos como <code>required</code> y <code>placeholder</code>.</p>",
                    Orden = 3,
                    DuracionMinutos = 25,
                    CursoId = cursos[0].Id
                },

                // CSS3
                new Leccion
                {
                    Titulo = "Selectores avanzados",
                    Contenido = "<h2>Selectores avanzados</h2><p>CSS3 permite seleccionar elementos con precisión: <code>:nth-child()</code>, <code>::before</code>, <code>::after</code>, <code>[atributo]</code> y combinadores como <code>+</code> y <code>~</code>.</p>",
                    Orden = 1,
                    DuracionMinutos = 20,
                    CursoId = cursos[1].Id
                },
                new Leccion
                {
                    Titulo = "Flexbox y Grid",
                    Contenido = "<h2>Flexbox y CSS Grid</h2><p><strong>Flexbox</strong> es ideal para layouts unidimensionales. <strong>CSS Grid</strong> maneja layouts bidimensionales con filas y columnas simultáneamente.</p>",
                    Orden = 2,
                    DuracionMinutos = 30,
                    CursoId = cursos[1].Id
                },
                new Leccion
                {
                    Titulo = "Animaciones y transiciones",
                    Contenido = "<h2>Animaciones y transiciones</h2><p>Las transiciones suavizan cambios de propiedades. Las animaciones con <code>@keyframes</code> permiten secuencias complejas con múltiples pasos.</p>",
                    Orden = 3,
                    DuracionMinutos = 25,
                    CursoId = cursos[1].Id
                },

                // JavaScript
                new Leccion
                {
                    Titulo = "Variables: let, const y var",
                    Contenido = "<h2>Variables en JavaScript moderno</h2><p><code>let</code> para variables mutables con scope de bloque. <code>const</code> para referencias inmutables. <code>var</code> está obsoleto por su scope de función y hoisting.</p>",
                    Orden = 1,
                    DuracionMinutos = 15,
                    CursoId = cursos[2].Id
                },
                new Leccion
                {
                    Titulo = "Arrow functions y this",
                    Contenido = "<h2>Arrow functions</h2><p>Las arrow functions (<code>=&gt;</code>) tienen sintaxis concisa y no vinculan su propio <code>this</code>, heredándolo del contexto donde se definen.</p>",
                    Orden = 2,
                    DuracionMinutos = 20,
                    CursoId = cursos[2].Id
                },
                new Leccion
                {
                    Titulo = "Promesas y async/await",
                    Contenido = "<h2>Promesas y async/await</h2><p>Las promesas representan operaciones asíncronas. <code>async/await</code> permite escribir código asíncrono que se lee como síncrono.</p>",
                    Orden = 3,
                    DuracionMinutos = 30,
                    CursoId = cursos[2].Id
                }
            };

            await contexto.Lecciones.AddRangeAsync(lecciones);
            await contexto.SaveChangesAsync();
        }
    }
}
