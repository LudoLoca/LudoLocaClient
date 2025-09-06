using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Client.Models;

namespace Client.Controllers
{
    //[Authorize(Roles = "Admin")] // Apenas administradores podem acessar este controller. Passei esse controle para o método RedirectToRefererIfIsSimpleUser(), chamado dentro do método GET;
    public class UsersController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;

        public UsersController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        private IActionResult? RedirectToRefererIfIsSimpleUser()
        {
            if (!User.IsInRole("Admin"))
            {
                var referer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referer))
                    return Redirect(referer);

                return RedirectToAction("Index", "Home"); // fallback
            }

            return null; // significa que é admin e pode seguir normalmente
        }


        // Exibe a lista de usuários (GET: /Users)
        public async Task<IActionResult> Index()
        {
            if (RedirectToRefererIfIsSimpleUser() is IActionResult check)
                return check;

            var client = _httpFactory.CreateClient("Api");
            var users = await client.GetFromJsonAsync<List<UserViewModel>>("api/users");
            return View(users ?? new List<UserViewModel>());
        }

        // Remove um usuário pelo id (POST: /Users/Delete/{id})
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var client = _httpFactory.CreateClient("Api");
            var resp = await client.DeleteAsync($"api/users/{id}");
            if (!resp.IsSuccessStatusCode)
                TempData["Error"] = "Failed to delete user.";
            return RedirectToAction(nameof(Index));
        }

        // Define ou remove o papel de admin de um usuário (POST: /Users/SetAdmin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetAdmin(Guid id, bool isAdmin)
        {
            var client = _httpFactory.CreateClient("Api");
            var content = new StringContent(JsonSerializer.Serialize(isAdmin), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync($"api/users/{id}/set-admin", content);
            if (!resp.IsSuccessStatusCode)
                TempData["Error"] = "Failed to update admin status.";
            return RedirectToAction(nameof(Index));
        }

    }
}
