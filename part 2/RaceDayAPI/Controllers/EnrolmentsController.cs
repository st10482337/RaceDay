using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // ============================================
        // GET: api/Enrolments
        // Participants view their own enrolments
        // ============================================
        [HttpGet]
        public async Task<IActionResult> GetMyEnrolments()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Participant")
            {
                return StatusCode(
                    403,
                    "Only participants can view their enrolments."
                );
            }

            var enrolments = await _context.Enrolments
                .AsNoTracking()
                .Where(e => e.ParticipantID == userId.Value)
                .Select(e => new
                {
                    e.EnrolmentID,
                    e.ParticipantID,
                    e.EventID,
                    EventName = e.Event != null
                        ? e.Event.EventName
                        : null,
                    e.CategoryID,
                    CategoryName = e.Category != null
                        ? e.Category.CategoryName
                        : null,
                    e.EnrolmentDate,
                    e.Status
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        // ============================================
        // POST: api/Enrolments
        // Participants enrol in an event
        // ============================================
        [HttpPost]
        public async Task<IActionResult> CreateEnrolment(
            [FromBody] CreateEnrolmentDto request)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Participant")
            {
                return StatusCode(
                    403,
                    "Only participants can enrol in events."
                );
            }

            // Check whether the event exists
            var eventRecord = await _context.Events
                .FindAsync(request.EventID);

            if (eventRecord == null)
            {
                return NotFound("Event not found.");
            }

            // Prevent enrolment in past events
            if (eventRecord.EventDate <= DateTime.Now)
            {
                return BadRequest(
                    "You cannot enrol in an event that has already started."
                );
            }

            // Check whether the category exists
            var category = await _context.Categories
                .FindAsync(request.CategoryID);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            // Make sure the category belongs to the selected event
            if (category.EventID != request.EventID)
            {
                return BadRequest(
                    "The selected category does not belong to this event."
                );
            }

            // Prevent duplicate enrolments
            bool alreadyEnrolled = await _context.Enrolments
                .AnyAsync(e =>
                    e.ParticipantID == userId.Value &&
                    e.EventID == request.EventID);

            if (alreadyEnrolled)
            {
                return Conflict(
                    "You have already enrolled in this event."
                );
            }

            // Create the enrolment
            var enrolment = new Enrolment
            {
                ParticipantID = userId.Value,
                EventID = request.EventID,
                CategoryID = request.CategoryID,
                EnrolmentDate = DateTime.Now,
                Status = "Enrolled"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                Message = "Enrolment created successfully.",
                enrolment.EnrolmentID,
                enrolment.ParticipantID,
                enrolment.EventID,
                enrolment.CategoryID,
                enrolment.EnrolmentDate,
                enrolment.Status
            });
        }

        // ============================================
        // DELETE: api/Enrolments/{id}
        // Participants cancel their enrolment
        // ============================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelEnrolment(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Participant")
            {
                return StatusCode(
                    403,
                    "Only participants can cancel enrolments."
                );
            }

            var enrolment = await _context.Enrolments
                .FindAsync(id);

            if (enrolment == null)
            {
                return NotFound("Enrolment not found.");
            }

            // Participants can only cancel their own enrolments
            if (enrolment.ParticipantID != userId.Value)
            {
                return StatusCode(
                    403,
                    "You can only cancel your own enrolments."
                );
            }

            // Do not delete an enrolment that already has a result
            bool hasResult = await _context.Results
                .AnyAsync(r => r.EnrolmentID == id);

            if (hasResult)
            {
                return Conflict(
                    "Cannot cancel an enrolment that already has a result."
                );
            }

            _context.Enrolments.Remove(enrolment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Enrolment cancelled successfully.",
                EnrolmentID = id
            });
        }
    }

    // ============================================
    // DTO: Information needed to enrol
    // ============================================
    public class CreateEnrolmentDto
    {
        [Required]
        public int EventID { get; set; }

        [Required]
        public int CategoryID { get; set; }
    }
}