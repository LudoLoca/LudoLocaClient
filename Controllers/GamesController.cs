using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Client.Models;

namespace Client.Controllers
{
    [Authorize(Roles = "Admin")] // Apenas administradores podem acessar este controller
    public class GamesController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;
        public GamesController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }
        // GET: GameController
        public async Task<IActionResult> Index()
        {
            var client = _httpFactory.CreateClient("Api");
            var games = await client.GetFromJsonAsync<List<GameViewModel>>("api/Game");
            return View(games ?? new List<GameViewModel>());
        }

        // GET: GameController/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var client = _httpFactory.CreateClient("Api");
            var game = await client.GetFromJsonAsync<GameViewModel>($"api/Game/{id}");
            if (game == null)
                return NotFound();
            return View(game);
        }

        // GET: GameController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GameController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameViewModel game)
        {
            if (!ModelState.IsValid)
                return View(game);

            var client = _httpFactory.CreateClient("Api");
            var response = await client.PostAsJsonAsync("api/Game", game);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            TempData["Error"] = "Não foi possível criar o jogo.";
            return View(game);
        }

        // GET: GamesController/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = _httpFactory.CreateClient("Api");
            var game = await client.GetFromJsonAsync<GameViewModel>($"api/Game/{id}");
            if (game == null)
                return NotFound();

            return View(game);
        }

        // POST: GameController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, GameViewModel game)
        {
            if (!ModelState.IsValid)
                return View(game);

            var client = _httpFactory.CreateClient("Api");
            var response = await client.PutAsJsonAsync($"api/Game/{id}", game);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            TempData["Error"] = "Não foi possível editar o jogo.";
            return View(game);
        }

        // GET: GameController/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var client = _httpFactory.CreateClient("Api");
            var game = await client.GetFromJsonAsync<GameViewModel>($"api/Game/{id}");
            if (game == null)
                return NotFound();

            return View(game);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var client = _httpFactory.CreateClient("Api");
            var response = await client.DeleteAsync($"api/Game/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            TempData["Error"] = "Não foi possível excluir o jogo.";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}
