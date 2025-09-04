using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Client.Models;

namespace Client.Controllers
{
    [Authorize(Roles = "Admin")] // Apenas administradores podem acessar este controller
    public class GenreController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;

        public GenreController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        // GET: GenreController
        public async Task<IActionResult> Index()
        {
            var client = _httpFactory.CreateClient("Api");
            var genres = await client.GetFromJsonAsync<List<GenreViewModel>>("api/Genre");
            return View(genres ?? new List<GenreViewModel>());
        }

        // GET: GenreController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GenreController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GenreViewModel genre)
        {
            if (!ModelState.IsValid)
                return View(genre);

            var client = _httpFactory.CreateClient("Api");
            var response = await client.PostAsJsonAsync("api/Genre", genre);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            TempData["Error"] = "Não foi possível criar o gênero.";
            return View(genre);
        }

        // GET: GenreController/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = _httpFactory.CreateClient("Api");
            var genre = await client.GetFromJsonAsync<GenreViewModel>($"api/Genre/{id}");
            if (genre == null)
                return NotFound();

            return View(genre);
        }

        // POST: GenreController/Edit/5 // DEBUGAR!!!!!!!!!!!!!!!!!!
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, GenreViewModel genre)
        {   
            if (!ModelState.IsValid)
                return View(genre);

            var client = _httpFactory.CreateClient("Api");
            var response = await client.PatchAsJsonAsync($"api/Genre/{id}", genre);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            TempData["Error"] = "Não foi possível editar o gênero.";
            return View(genre);
        }

        // GET: GenreController/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var client = _httpFactory.CreateClient("Api");
            var genre = await client.GetFromJsonAsync<GenreViewModel>($"api/Genre/{id}");
            if (genre == null)
                return NotFound();

            return View(genre);
        }

        // POST: GenreController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var client = _httpFactory.CreateClient("Api");
            var response = await client.DeleteAsync($"api/Genre/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            TempData["Error"] = "Não foi possível excluir o gênero.";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}
