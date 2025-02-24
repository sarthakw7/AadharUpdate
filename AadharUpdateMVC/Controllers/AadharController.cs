using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AadharUpdateMVC.Models;


namespace AadharUpdateMVC.Controllers
{
    public class AadharController : Controller
    {
        private readonly HttpClient _httpClient;

        public AadharController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7200/api/Aadhar");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("");

                var jsonResponse = await response.Content.ReadAsStringAsync();

                var data = JsonSerializer.Deserialize<List<AadharDetailsViewModel>>(jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

                return View(data);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = "Failed to load Aadhar Details";
                return View(new List<AadharDetailsViewModel>());
            }
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(AadharDetailsViewModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await _httpClient.PostAsync("", content);
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, "Error creating Aadhar details. Please try again.");
                return View(model);
            }

        }

        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/{id}");
                response.EnsureSuccessStatusCode();

                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = "Error deleting Aadhar details. Please try again.";
                return RedirectToAction("Index");
            }
        }
    }
}
