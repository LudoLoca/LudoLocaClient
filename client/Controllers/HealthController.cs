using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class HealthController : Controller
    {
        private readonly IHttpClientFactory _factory;
        public HealthController(IHttpClientFactory factory) => _factory = factory;

        public async Task<IActionResult> Index()
        {
            var http = _factory.CreateClient("Api");
            try
            {
                var resp = await http.GetFromJsonAsync<object>("api/dbping");
                ViewBag.ApiStatus = "OK";
                ViewBag.ApiPayload = resp;
            }
            catch (Exception ex)
            {
                ViewBag.ApiStatus = "ERROR";
                ViewBag.ApiPayload = ex.Message;
            }
            return View();
        }
    }
}
