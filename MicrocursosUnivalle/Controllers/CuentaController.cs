using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MicrocursosUnivalle.ModelosVista;

namespace MicrocursosUnivalle.Controllers
{
    public class CuentaController : Controller
    {
        private readonly UserManager<IdentityUser> _gestorUsuarios;
        private readonly SignInManager<IdentityUser> _gestorSesion;

        public CuentaController(UserManager<IdentityUser> gestorUsuarios, SignInManager<IdentityUser> gestorSesion)
        {
            _gestorUsuarios = gestorUsuarios;
            _gestorSesion = gestorSesion;
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            if (_gestorSesion.IsSignedIn(User)) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistrarVM modelo)
        {
            if (!ModelState.IsValid) return View(modelo);

            var nuevoUsuario = new IdentityUser
            {
                UserName = modelo.Correo,
                Email = modelo.Correo,
                EmailConfirmed = true
            };

            var resultado = await _gestorUsuarios.CreateAsync(nuevoUsuario, modelo.Contrasena);
            if (resultado.Succeeded)
            {
                await _gestorUsuarios.AddToRoleAsync(nuevoUsuario, "Suscriptor");
                await _gestorSesion.SignInAsync(nuevoUsuario, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in resultado.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(modelo);
        }

        [HttpGet]
        public IActionResult Ingresar(string? returnUrl = null)
        {
            if (_gestorSesion.IsSignedIn(User)) return RedirectToAction("Index", "Home");
            ViewData["UrlRetorno"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ingresar(IngresarVM modelo, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(modelo);

            var resultado = await _gestorSesion.PasswordSignInAsync(
                modelo.Correo, modelo.Contrasena, modelo.Recordarme, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Salir()
        {
            await _gestorSesion.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Denegado()
        {
            return View();
        }
    }
}
