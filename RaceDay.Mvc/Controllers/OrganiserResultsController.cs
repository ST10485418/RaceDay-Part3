
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Mvc.Controllers
{
    public class OrganiserResultsController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public OrganiserResultsController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        private IActionResult? CheckAccess()
        {
            string? token = HttpContext.Session.GetString("AccessToken");
            string? role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Account");

            if (!string.Equals(role, "Organiser",
                StringComparison.OrdinalIgnoreCase))
                return StatusCode(403);

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var denied = CheckAccess();
            if (denied != null)
                return denied;

            var client = _factory.CreateClient("RaceDayApi");

            string? token = HttpContext.Session.GetString("AccessToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                var results = await client.GetFromJsonAsync
                    <List<OrganiserResultModel>>("api/Results");

                return View(results ?? new List<OrganiserResultModel>());
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to load results from RaceDay API.";
                return View(new List<OrganiserResultModel>());
            }
        }
    }

    public class OrganiserResultModel
    {
        public int ResultID { get; set; }
        public int EnrolmentID { get; set; }
        public TimeSpan? ChipTime { get; set; }
        public TimeSpan? GunTime { get; set; }
        public int? PositionOverall { get; set; }
        public int? PositionCategory { get; set; }
        public bool IsCompleted { get; set; }
        public string? Notes { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
