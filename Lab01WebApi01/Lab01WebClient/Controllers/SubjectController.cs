using Lab01WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Lab01WebClient.Controllers
{
    public class SubjectController : Controller
    {
        private readonly string url = "https://localhost:7240/api/Subject";
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<IActionResult> Index()
        {
            var model = await _httpClient.GetFromJsonAsync<List<Subject>>(url);
            return View(model ?? new List<Subject>());
        }

        // GET: /Subject/Details/{code}
        public async Task<IActionResult> Details(string code)
        {
            var subject = await _httpClient.GetFromJsonAsync<Subject>($"{url}/{code}");
            return View(subject);
        }

        // GET: /Subject/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Subject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subject subject)
        {
            try
            {
                await _httpClient.PostAsJsonAsync(url, subject);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(subject);   
            }
        }

        // GET: /Subject/Delete/{code}
        public async Task<IActionResult> Delete(string code)
        {
            var subject = await _httpClient.GetFromJsonAsync<Subject>($"{url}/{code}");
            return View(subject);
        }

        // POST: /Subject/Delete/{code}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string code)
        {
            await _httpClient.DeleteAsync($"{url}/{code}");
            return RedirectToAction(nameof(Index));
        }
    }
}