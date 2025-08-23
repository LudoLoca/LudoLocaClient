using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Client.Models;

namespace Client.Controllers
{
    [Route("[controller]")] // Garante rotas do tipo /Account/...
    public class AccountController : Controller
    {
        private const string AuthScheme = "AppCookie"; // Deve coincidir com o Program.cs
        private readonly IHttpClientFactory _httpFactory;

        public AccountController(IHttpClientFactory httpFactory) => _httpFactory = httpFactory;

        // ===== REGISTRO DE USUÁRIO =====
        [HttpGet("Register")]
        [AllowAnonymous]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost("Register")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Validação básica do formulário
            if (!ModelState.IsValid) return View(model);
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View(model);
            }

            var client = _httpFactory.CreateClient("Api");

            try
            {
                // Envia requisição para registrar usuário na API
                var resp = await client.PostAsJsonAsync("api/account/register", new
                {
                    Email = model.Email,
                    Password = model.Password
                });

                if (resp.IsSuccessStatusCode)
                {
                    TempData["Flash"] = "Registration successful. Please log in.";
                    return RedirectToAction("Login");
                }

                // Exibe erro retornado pela API
                var apiErr = await SafeReadText(resp);
                ModelState.AddModelError("", $"Registration failed: {apiErr}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Registration service unavailable: {ex.Message}");
                return View(model);
            }
        }

        // ===== LOGIN DE USUÁRIO =====
        [HttpGet("Login")]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpFactory.CreateClient("Api");

            try
            {
                // Envia requisição de login para a API
                var resp = await client.PostAsJsonAsync("api/account/login", new
                {
                    Email = model.Email,
                    Password = model.Password,
                    RememberMe = model.RememberMe
                });

                if (!resp.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }

                // Lê resposta da API (usuário, papéis, etc.)
                var result = await SafeRead<LoginResult>(resp);
                var email = result?.email ?? model.Email;
                var userId = result?.userId ?? email;
                var roles = result?.roles ?? Array.Empty<string>();

                // Cria cookie de autenticação para o MVC
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, email)
                };
                foreach (var r in roles) claims.Add(new Claim(ClaimTypes.Role, r));

                var identity = new ClaimsIdentity(claims, AuthScheme);
                await HttpContext.SignInAsync(
                    AuthScheme,
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties { IsPersistent = model.RememberMe });

                return RedirectToLocal(returnUrl);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Login service unavailable. Try again.");
                return View(model);
            }
        }

        // ===== LOGOUT =====
        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Remove o cookie de autenticação do MVC
            await HttpContext.SignOutAsync(AuthScheme);
            return RedirectToAction("Index", "Home");
        }

        // ===== Helpers =====

        // Redireciona para URL local ou para Home
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        // Lê texto de resposta HTTP de forma segura
        private static async Task<string> SafeReadText(HttpResponseMessage resp)
        {
            try { return await resp.Content.ReadAsStringAsync(); }
            catch { return resp.ReasonPhrase ?? "Unknown error"; }
        }

        // Lê objeto JSON de resposta HTTP de forma segura
        private static async Task<T?> SafeRead<T>(HttpResponseMessage resp) where T : class
        {
            try { return await resp.Content.ReadFromJsonAsync<T>(); }
            catch { return null; }
        }

        // Modelo para resposta de login da API
        private sealed class LoginResult
        {
            public bool success { get; set; }
            public string? userId { get; set; }
            public string? email { get; set; }
            public string[]? roles { get; set; }
        }
    }
}
