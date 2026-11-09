
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Mvc.Controllers
{
    public class ManageCategoriesController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public ManageCategoriesController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        private IActionResult? CheckAccess()
        {
            var token = HttpContext.Session.GetString("AccessToken");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Account");

            if (!string.Equals(role, "Organiser",
                StringComparison.OrdinalIgnoreCase))
                return StatusCode(403);

            return null;
        }

        private HttpClient GetClient()
        {
            var client = _factory.CreateClient("RaceDayApi");
            var token = HttpContext.Session.GetString("AccessToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var denied = CheckAccess();
            if (denied != null) return denied;

            using var client = GetClient();

            try
            {
                var categories = await client.GetFromJsonAsync
                    <List<ManageCategoryModel>>("api/Categories");

                return View(categories ?? new List<ManageCategoryModel>());
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Could not load categories.";
                return View(new List<ManageCategoryModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            var denied = CheckAccess();
            if (denied != null) return denied;

            return View(new ManageCategoryModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ManageCategoryModel model)
        {
            var denied = CheckAccess();
            if (denied != null) return denied;

            if (!ModelState.IsValid) return View(model);

            using var client = GetClient();

            try
            {
                using var response = await client.PostAsJsonAsync(
                    "api/Categories",
                    new
                    {
                        model.CategoryName,
                        model.CategoryType,
                        model.Description
                    });

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Category created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", await GetError(response));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "Could not connect to API.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var denied = CheckAccess();
            if (denied != null) return denied;

            using var client = GetClient();

            try
            {
                using var response =
                    await client.GetAsync($"api/Categories/{id}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return NotFound();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode);

                var model = await response.Content
                    .ReadFromJsonAsync<ManageCategoryModel>();

                if (model == null) return NotFound();

                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Could not connect to API.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, ManageCategoryModel model)
        {
            var denied = CheckAccess();
            if (denied != null) return denied;

            if (!ModelState.IsValid) return View(model);

            using var client = GetClient();

            try
            {
                using var response = await client.PutAsJsonAsync(
                    $"api/Categories/{id}",
                    new
                    {
                        model.CategoryName,
                        model.CategoryType,
                        model.Description
                    });

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Category updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", await GetError(response));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "Could not connect to API.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var denied = CheckAccess();
            if (denied != null) return denied;

            using var client = GetClient();

            try
            {
                using var response =
                    await client.DeleteAsync($"api/Categories/{id}");

                if (response.IsSuccessStatusCode)
                    TempData["Success"] = "Category deleted successfully!";
                else
                    TempData["Error"] = await GetError(response);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Could not connect to API.";
            }

            return RedirectToAction(nameof(Index));
        }

        private static async Task<string> GetError(
            HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return "Please log in again.";

            if (response.StatusCode == HttpStatusCode.Forbidden)
                return "Only Organisers can manage categories.";

            return await response.Content.ReadAsStringAsync();
        }
    }

    public class ManageCategoryModel
    {
        public int CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string CategoryType { get; set; } = "";

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
