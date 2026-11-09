
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IHttpClientFactory httpClientFactory,
            ILogger<AccountController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // ==========================================
        // LOGIN - GET
        // ==========================================
        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(
                HttpContext.Session.GetString("AccessToken")))
            {
                return RedirectToAction("Index", "Events");
            }

            return View(new LoginViewModel());
        }

        // ==========================================
        // LOGIN - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = _httpClientFactory
                    .CreateClient("RaceDayApi");

                using var response = await client.PostAsJsonAsync(
                    "api/auth/login",
                    new
                    {
                        model.Email,
                        model.Password
                    });

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(
                        "",
                        response.StatusCode == HttpStatusCode.Unauthorized
                            ? "Invalid email or password."
                            : "Login failed. Please try again.");

                    return View(model);
                }

                using var json = await response.Content
                    .ReadFromJsonAsync<JsonDocument>();

                if (json == null)
                {
                    throw new JsonException(
                        "Empty login response.");
                }

                var root = json.RootElement;

                string token = root
                    .GetProperty("accessToken")
                    .GetString() ?? "";

                var user = root.GetProperty("user");

                string role = user.GetProperty("role")
                    .GetString() ?? "";

                string firstName = user
                    .GetProperty("firstName")
                    .GetString() ?? "";

                int userId = user
                    .GetProperty("userID")
                    .GetInt32();

                if (string.IsNullOrWhiteSpace(token) ||
                    (role != "Organiser" &&
                     role != "Participant"))
                {
                    throw new JsonException(
                        "Invalid login response.");
                }

                // Clear any previous session data
                HttpContext.Session.Clear();

                // Store logged-in user information
                HttpContext.Session.SetString(
                    "AccessToken", token);

                HttpContext.Session.SetString(
                    "Role", role);

                HttpContext.Session.SetString(
                    "FirstName", firstName);

                HttpContext.Session.SetInt32(
                    "UserID", userId);

                // This is a temporary local expiry value.
                // We will validate the JWT's actual expiry
                // when implementing protected pages.
                HttpContext.Session.SetString(
                    "TokenExpiresAt",
                    DateTimeOffset.UtcNow
                        .AddMinutes(60)
                        .ToString("O"));

                TempData["SuccessMessage"] =
                    "Welcome to RaceDay, " + firstName + "!";

                return RedirectToAction("Index", "Events");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex, "API login connection failed.");

                ModelState.AddModelError(
                    "",
                    "Unable to connect to RaceDay. Try again.");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(
                    ex, "API login timed out.");

                ModelState.AddModelError(
                    "",
                    "Login timed out. Please try again.");
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex, "Invalid API login response.");

                ModelState.AddModelError(
                    "",
                    "Unable to complete login.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(
                    ex, "Missing API login response field.");

                ModelState.AddModelError(
                    "",
                    "Unable to complete login.");
            }

            return View(model);
        }

        // ==========================================
        // REGISTER - GET
        // ==========================================
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // ==========================================
        // REGISTER - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = _httpClientFactory
                    .CreateClient("RaceDayApi");

                using var response = await client.PostAsJsonAsync(
                    "api/auth/register",
                    new
                    {
                        model.FirstName,
                        model.LastName,
                        model.Email,
                        model.Password,
                        model.Phone
                    });

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Registration successful! Please log in.";

                    return RedirectToAction(
                        "Login", "Account");
                }

                if (response.StatusCode ==
                    HttpStatusCode.Conflict)
                {
                    ModelState.AddModelError(
                        "Email",
                        "This email is already registered.");
                }
                else
                {
                    ModelState.AddModelError(
                        "",
                        "Registration failed. Please check your details.");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex, "API registration connection failed.");

                ModelState.AddModelError(
                    "",
                    "Unable to connect to RaceDay. Try again.");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(
                    ex, "API registration timed out.");

                ModelState.AddModelError(
                    "",
                    "Registration timed out. Please try again.");
            }

            return View(model);
        }

        // ==========================================
        // LOGOUT - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            TempData["SuccessMessage"] =
                "You have logged out successfully.";

            return RedirectToAction(
                "Login", "Account");
        }
    }
}
