using Client.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Client.Controllers
{
    [Authorize(Roles = "Admin")] // Apenas administradores podem acessar este controller
    public class GameListingController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;
        public GameListingController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        // GET: GameController
        public async Task<IActionResult> Index()
        {
            var client = _httpFactory.CreateClient("Api");
            var gameListings = await client.GetFromJsonAsync<List<GameListingViewModel>>("api/GameListing");

            return View(gameListings ?? new List<GameListingViewModel>());
        }

        // GET: GameController/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var client = _httpFactory.CreateClient("Api");
            var gameListing = await client.GetFromJsonAsync<GameListingViewModel>($"api/GameListing/{id}");
            if (gameListing == null)
                return NotFound();
            return View(gameListing);
        }

        // GET: GameController/Create
        public async Task<IActionResult> Create()
        {
            var client = _httpFactory.CreateClient("Api");

            var availableGames = await client.GetFromJsonAsync<List<GameViewModel>>("api/Game") ?? new();

            var vm = new GameListingCreateViewModel
            {
                AvailableGames = availableGames
            };

            return View(vm);
        }

        // POST: GameController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameListingViewModel gameListing)
        {
            if (!ModelState.IsValid)
                return View(gameListing);

            var client = _httpFactory.CreateClient("Api");
            var response = await client.PostAsJsonAsync("api/GameListing", gameListing);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            TempData["Error"] = "Não foi possível criar o aluguel.";
            return View(gameListing);
        }


        // GET: GamesController/Edit/5
        //public async Task<IActionResult> Edit(Guid id)
        //{
        //    var client = _httpFactory.CreateClient("Api");
        //    var game = await client.GetFromJsonAsync<GameViewModel>($"api/Game/{id}");
        //    if (game == null)
        //        return NotFound();

        //    // carrega todos os gêneros
        //    var allGenres = await client.GetFromJsonAsync<List<GenreViewModel>>("api/Genre");
        //    game.AllGenres = allGenres ?? new();

        //    return View(game);
        //}

        // GET: GamesController/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
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