
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Controllers
{
    public class EventsController : Controller
    {
        private readonly IHttpClientFactory
            _httpClientFactory;

        private readonly ILogger<EventsController>
            _logger;

        public EventsController(
            IHttpClientFactory httpClientFactory,
            ILogger<EventsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var events = new List<EventViewModel>();

            try
            {
                var client = _httpClientFactory
                    .CreateClient("RaceDayApi");

                using var response = await client
                    .GetAsync("api/Events");

                if (response.IsSuccessStatusCode)
                {
                    events = await response.Content
                        .ReadFromJsonAsync<
                            List<EventViewModel>>()
                        ?? new List<EventViewModel>();
                }
                else
                {
                    _logger.LogWarning(
                        "API returned status {StatusCode}",
                        response.StatusCode);

                    ViewBag.ErrorMessage =
                        "Events are temporarily unavailable.";
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    "Could not connect to RaceDay API.");

                ViewBag.ErrorMessage =
                    "Unable to load events. Please try again.";
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex,
                    "RaceDay API request timed out.");

                ViewBag.ErrorMessage =
                    "The request timed out. Please try again.";
            }
            catch (System.Text.Json.JsonException ex)
            {
                _logger.LogError(ex,
                    "Invalid event data from RaceDay API.");

                ViewBag.ErrorMessage =
                    "Unable to display event information.";
            }

            return View(events);
        }
    }
}
