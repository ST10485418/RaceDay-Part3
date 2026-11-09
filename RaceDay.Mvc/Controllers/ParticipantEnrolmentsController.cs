
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Mvc.Controllers
{
    public class ParticipantEnrolmentsController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public ParticipantEnrolmentsController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        private IActionResult? CheckAccess()
        {
            string? token = HttpContext.Session.GetString("AccessToken");
            string? role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Account");

            if (!string.Equals(role, "Participant",
                StringComparison.OrdinalIgnoreCase))
                return StatusCode(403);

            return null;
        }

        private HttpClient GetClient()
        {
            var client = _factory.CreateClient("RaceDayApi");

            string? token = HttpContext.Session.GetString("AccessToken");

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
                var enrolments = await client.GetFromJsonAsync
                    <List<ParticipantEnrolmentModel>>("api/Enrolments/mine");

                return View(enrolments ?? new List<ParticipantEnrolmentModel>());
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to load your enrolments.";
                return View(new List<ParticipantEnrolmentModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var denied = CheckAccess();
            if (denied != null) return denied;

            using var client = GetClient();

            try
            {
                var categories = await client.GetFromJsonAsync
                    <List<ParticipantEventCategoryModel>>("api/EventCategories");

                return View(categories ?? new List<ParticipantEventCategoryModel>());
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to load event categories.";
                return View(new List<ParticipantEventCategoryModel>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enrol(int eventCategoryId)
        {
            var denied = CheckAccess();
            if (denied != null) return denied;

            using var client = GetClient();

            try
            {
                using var response = await client.PostAsJsonAsync(
                    "api/Enrolments",
                    new { EventCategoryID = eventCategoryId });

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Enrolment submitted successfully!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = await ReadError(response);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to RaceDay API.";
            }

            return RedirectToAction(nameof(Create));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var denied = CheckAccess();
            if (denied != null) return denied;

            using var client = GetClient();

            try
            {
                using var response = await client.PutAsync(
                    $"api/Enrolments/{id}/cancel", null);

                if (response.IsSuccessStatusCode)
                    TempData["Success"] = "Enrolment cancelled.";
                else
                    TempData["Error"] = await ReadError(response);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to RaceDay API.";
            }

            return RedirectToAction(nameof(Index));
        }

        private static async Task<string> ReadError(
            HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return "Your session has expired. Please log in again.";

            if (response.StatusCode == HttpStatusCode.Forbidden)
                return "Participant access is required.";

            return await response.Content.ReadAsStringAsync();
        }
    }

    public class ParticipantEnrolmentModel
    {
        public int EnrolmentID { get; set; }
        public int UserID { get; set; }
        public int EventCategoryID { get; set; }
        public DateTime EnrolmentDate { get; set; }
        public string Status { get; set; } = "";
    }

    public class ParticipantEventCategoryModel
    {
        public int EventCategoryID { get; set; }
        public int EventID { get; set; }
        public int CategoryID { get; set; }
        public decimal EntryFee { get; set; }
        public int MaxEntries { get; set; }
        public bool IsActive { get; set; }
    }
}
