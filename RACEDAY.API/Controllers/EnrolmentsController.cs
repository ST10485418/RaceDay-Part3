
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Participant")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public EnrolmentsController(RaceDayContext context)
        {
            _context = context;
        }

        [HttpGet("mine")]
        public async Task<IActionResult> MyEnrolments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid participant identity.");

            var enrolments = await _context.Enrolments
                .Where(e => e.UserID == userId)
                .ToListAsync();

            return Ok(enrolments);
        }

        [HttpPost]
        public async Task<IActionResult> Enrol([FromBody] EnrolRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid participant identity.");

            var category = await _context.EventCategories
                .FirstOrDefaultAsync(x =>
                    x.EventCategoryID == request.EventCategoryID);

            if (category == null)
                return NotFound("Event category not found.");

            if (!category.IsActive)
                return BadRequest("This event category is not active.");

            var existing = await _context.Enrolments
                .FirstOrDefaultAsync(e =>
                    e.UserID == userId &&
                    e.EventCategoryID == request.EventCategoryID);

            if (existing != null)
                return BadRequest("You are already enrolled in this category.");

            var enrolment = new Enrolment
            {
                UserID = userId,
                EventCategoryID = request.EventCategoryID,
                EnrolmentDate = DateTime.Now,
                Status = "Pending"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return Ok(enrolment);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var enrolment = await _context.Enrolments
                .FirstOrDefaultAsync(e =>
                    e.EnrolmentID == id && e.UserID == userId);

            if (enrolment == null)
                return NotFound();

            enrolment.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return Ok(enrolment);
        }
    }

    public class EnrolRequest
    {
        public int EventCategoryID { get; set; }
    }
}
