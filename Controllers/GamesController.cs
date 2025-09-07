using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Client.Models;

namespace Client.Controllers
{
    //[Authorize(Roles = "Admin")] // Apenas administradores podem acessar este controller
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
            var isAdmin = User.IsInRole("Admin");

            ViewBag.IsAdmin = isAdmin;

            return View(games ?? new List<GameViewModel>());
        }

        // GET: GameController/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var client = _httpFactory.CreateClient("Api");
            var game = await client.GetFromJsonAsync<GameViewModel>($"api/Game/{id}");
            if (game == null)
                return NotFound();

            var isAdmin = User.IsInRole("Admin");
            ViewBag.IsAdmin = isAdmin;

            return View(game);
        }

        // GET: GameController/Create
        public async Task<IActionResult> Create()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            var client = _httpFactory.CreateClient("Api");
            var allGenres = await client.GetFromJsonAsync<List<GenreViewModel>>("api/Genre");
            var vm = new GameViewModel
            {
                AllGenres = allGenres ?? new List<GenreViewModel>()
            };

            return View(vm);
        }

        // POST: GameController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameViewModel game, List<Guid> SelectedGenres)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
                return View(game);

            var client = _httpFactory.CreateClient("Api");

            // 1) Cria o jogo
            var response = await client.PostAsJsonAsync("api/Game", game);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Não foi possível criar o jogo.";
                game.AllGenres = await client.GetFromJsonAsync<List<GenreViewModel>>("api/Genre") ?? new();
                return View(game);
            }

            // 2) Recupera o jogo criado para pegar o Id
            var createdGame = await response.Content.ReadFromJsonAsync<GameViewModel>();
            if (createdGame == null || createdGame.Id == Guid.Empty)
            {
                TempData["Error"] = "Erro ao recuperar o jogo recém-criado.";
                return RedirectToAction(nameof(Index));
            }

            // 3) Cria as associações com os gêneros
            foreach (var gid in SelectedGenres)
            {
                var req = new { GameId = createdGame.Id, GenreId = gid };
                await client.PostAsJsonAsync("api/GenreGame", req);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: GamesController/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            var client = _httpFactory.CreateClient("Api");

            // busca o jogo
            var game = await client.GetFromJsonAsync<GameViewModel>($"api/Game/{id}");
            if (game == null)
                return NotFound();

            // busca todos os gêneros
            var allGenres = await client.GetFromJsonAsync<List<GenreViewModel>>("api/Genre");
            game.AllGenres = allGenres ?? new();

            // busca os gêneros já vinculados ao jogo
            var linkedGenres = await client.GetFromJsonAsync<List<GenreViewModel>>($"api/GenreGame/{id}");
            game.Genres = linkedGenres ?? new();

            return View(game);
        }

        // POST: GameController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, GameViewModel game, List<Guid> SelectedGenres)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
                return View(game);

            var client = _httpFactory.CreateClient("Api");

            // Atualiza dados principais do jogo
            var response = await client.PatchAsJsonAsync($"api/Game/{id}", game);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Não foi possível editar o jogo.";
                game.AllGenres = await client.GetFromJsonAsync<List<GenreViewModel>>("api/Genre") ?? new();
                return View(game);
            }

            // 🔹 Atualiza os gêneros associados ao jogo
            // 1) Remove associações antigas
            var existingGenres = await client.GetFromJsonAsync<List<GenreViewModel>>($"api/GenreGame/{id}") ?? new();

            foreach (var eg in existingGenres)
                await client.DeleteAsync($"api/GenreGame/genre/{eg.Id}/game/{id}");

            // 2) Adiciona novas associações
            foreach (var gid in SelectedGenres)
            {
                var req = new { GameId = id, GenreId = gid };
                await client.PostAsJsonAsync("api/GenreGame", req);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: GameController/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

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
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            var client = _httpFactory.CreateClient("Api");
            var response = await client.DeleteAsync($"api/Game/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            TempData["Error"] = "Não foi possível excluir o jogo.";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}
