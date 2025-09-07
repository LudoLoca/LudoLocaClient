using Client.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Client.Controllers
{
    public class GameListingController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;
        public GameListingController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        // GET: GameListingController | /api/GameListing
        public async Task<IActionResult> Index()
        {
            var client = _httpFactory.CreateClient("Api");
            var gameListings = await client.GetFromJsonAsync<List<GameListingViewModel>>("api/GameListing");
            var isAdmin = User.IsInRole("Admin");

            ViewBag.IsAdmin = isAdmin;

            return View(gameListings ?? new List<GameListingViewModel>());
        }

        // FALTA IMPLEMENTAR
        // GET: GameListingController/Details
        //public async Task<IActionResult> Details(Guid id)
        //{
        //    var client = _httpFactory.CreateClient("Api");
        //    var gameListing = await client.GetFromJsonAsync<GameListingViewModel>($"api/GameListing/{id}");
        //    if (gameListing == null)
        //        return NotFound();
        //    return View(gameListing);
        //}
        

        // GET: GameListingController/Create
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

        // POST: GameListingController/Create | /api/GameListing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameListingCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var clientReload = _httpFactory.CreateClient("Api");
                vm.AvailableGames = await clientReload.GetFromJsonAsync<List<GameViewModel>>("api/Game") ?? new();
                return View(vm);
            }

            var client = _httpFactory.CreateClient("Api");

            var payload = new
            {
                GameId = vm.SelectedGameId,
                PricePerDay = vm.PricePerDay,
                ConditionNotes = vm.ConditionNotes,
                IsAvailable = vm.IsAvailable
            };

            var response = await client.PostAsJsonAsync("api/GameListing", payload);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            TempData["Error"] = "Não foi possível criar o game listing.";
            vm.AvailableGames = await client.GetFromJsonAsync<List<GameViewModel>>("api/Game") ?? new();
            return View(vm);
        }

        /* PARA USAR COMO REFERÊNCIA: */
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
        //public async Task<IActionResult> Edit(Guid id)
        //{
        //    var client = _httpFactory.CreateClient("Api");

        //    // busca o jogo
        //    var game = await client.GetFromJsonAsync<GameViewModel>($"api/Game/{id}");
        //    if (game == null)
        //        return NotFound();

        //    // busca todos os gêneros
        //    var allGenres = await client.GetFromJsonAsync<List<GenreViewModel>>("api/Genre");
        //    game.AllGenres = allGenres ?? new();

        //    // busca os gêneros já vinculados ao jogo
        //    var linkedGenres = await client.GetFromJsonAsync<List<GenreViewModel>>($"api/GenreGame/{id}");
        //    game.Genres = linkedGenres ?? new();

        //    return View(game);
        //}

        //// POST: GameController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, GameViewModel game, List<Guid> SelectedGenres)
        //{
        //    if (!ModelState.IsValid)
        //        return View(game);

        //    var client = _httpFactory.CreateClient("Api");

        //    // Atualiza dados principais do jogo
        //    var response = await client.PatchAsJsonAsync($"api/Game/{id}", game);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        TempData["Error"] = "Não foi possível editar o jogo.";
        //        game.AllGenres = await client.GetFromJsonAsync<List<GenreViewModel>>("api/Genre") ?? new();
        //        return View(game);
        //    }

        //    // 🔹 Atualiza os gêneros associados ao jogo
        //    // 1) Remove associações antigas
        //    var existingGenres = await client.GetFromJsonAsync<List<GenreViewModel>>($"api/GenreGame/{id}") ?? new();

        //    foreach (var eg in existingGenres)
        //        await client.DeleteAsync($"api/GenreGame/genre/{eg.Id}/game/{id}");

        //    // 2) Adiciona novas associações
        //    foreach (var gid in SelectedGenres)
        //    {
        //        var req = new { GameId = id, GenreId = gid };
        //        await client.PostAsJsonAsync("api/GenreGame", req);
        //    }

        //    return RedirectToAction(nameof(Index));
        //}

        //// GET: GameController/Delete/5
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var client = _httpFactory.CreateClient("Api");
        //    var game = await client.GetFromJsonAsync<GameViewModel>($"api/Game/{id}");
        //    if (game == null)
        //        return NotFound();

        //    return View(game);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var client = _httpFactory.CreateClient("Api");
        //    var response = await client.DeleteAsync($"api/Game/{id}");

        //    if (response.IsSuccessStatusCode)
        //        return RedirectToAction(nameof(Index));

        //    TempData["Error"] = "Não foi possível excluir o jogo.";
        //    return RedirectToAction(nameof(Delete), new { id });
        //}
    }
}