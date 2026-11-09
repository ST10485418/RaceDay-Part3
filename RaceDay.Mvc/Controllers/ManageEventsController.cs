
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Mvc.Controllers
{
    using RaceDay.Mvc.Filters;
    [SessionRoleAuthorize("Organiser")]
    public class ManageEventsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ManageEventsController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private HttpClient CreateApiClient()
        {
            var client = _httpClientFactory.CreateClient("RaceDayApi");

            string apiUrl = _configuration["RaceDayApi:BaseUrl"]
                ?? "https://localhost:7239/";

            client.BaseAddress = new Uri(apiUrl.TrimEnd('/') + "/");

            string? token = HttpContext.Session.GetString("AccessToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        public async Task<IActionResult> Index()
        {
            using var client = CreateApiClient();

            try
            {
                var events = await client.GetFromJsonAsync<
                    List<ManageEventModel>>("api/Events");

                return View(events ?? new List<ManageEventModel>());
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to load events.";
                return View(new List<ManageEventModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ManageEventModel
            {
                EventDate = DateTime.Today.AddDays(7),
                IsActive = true,
                EventType = "Run"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ManageEventModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = CreateApiClient();

            try
            {
                var response = await client.PostAsJsonAsync(
                    "api/Events", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Event created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(
                    "",
                    await GetErrorMessage(response));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    "", "Unable to connect to RaceDay API.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            using var client = CreateApiClient();

            try
            {
                var response = await client.GetAsync($"api/Events/{id}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return NotFound();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode);

                var model = await response.Content
                    .ReadFromJsonAsync<ManageEventModel>();

                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to RaceDay API.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, ManageEventModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = CreateApiClient();

            try
            {
                var response = await client.PutAsJsonAsync(
                    $"api/Events/{id}", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Event updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(
                    "", await GetErrorMessage(response));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    "", "Unable to connect to RaceDay API.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var client = CreateApiClient();

            try
            {
                var response = await client.DeleteAsync($"api/Events/{id}");

                if (response.IsSuccessStatusCode)
                    TempData["Success"] = "Event deleted successfully!";
                else
                    TempData["Error"] = await GetErrorMessage(response);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to RaceDay API.";
            }

            return RedirectToAction(nameof(Index));
        }

        private static async Task<string> GetErrorMessage(
            HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return "Your login token is missing or expired. Please log in again.";

            if (response.StatusCode == HttpStatusCode.Forbidden)
                return "You can only manage events that belong to your Organiser account.";

            return $"API returned {(int)response.StatusCode}: " +
                   await response.Content.ReadAsStringAsync();
        }
    }

    public class ManageEventModel
    {
        public int EventID { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string EventName { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Required]
        public string Description { get; set; } = "";

        public DateTime EventDate { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string Location { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Range(
            typeof(decimal), "0.01", "1000000")]
        public decimal DistanceKm { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string EventType { get; set; } = "Run";

        public bool IsActive { get; set; } = true;

        public int OrganiserID { get; set; }
    }
}
