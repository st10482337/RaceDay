using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // ============================================
        // GET: api/Events
        // View all events
        // ============================================
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .AsNoTracking()
                .Select(e => new
                {
                    e.EventID,
                    e.EventName,
                    e.Description,
                    e.EventDate,
                    e.Location,
                    e.Distance,
                    e.EventType,
                    e.OrganiserID
                })
                .ToListAsync();

            return Ok(events);
        }

        // ============================================
        // POST: api/Events
        // Organisers create events
        // ============================================
        [HttpPost]
        public async Task<IActionResult> CreateEvent(
            [FromBody] CreateEventDto request)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Organiser")
            {
                return StatusCode(
                    403,
                    "Only organisers can create events."
                );
            }

            if (request.EventDate <= DateTime.Now)
            {
                return BadRequest(
                    "Event date must be in the future."
                );
            }

            if (!IsValidEventType(request.EventType))
            {
                return BadRequest(
                    "EventType must be one of: Run, Walk, Cycle."
                );
            }

            var newEvent = new Event
            {
                EventName = request.EventName,
                Description = request.Description,
                EventDate = request.EventDate,
                Location = request.Location,
                Distance = request.Distance,
                EventType = request.EventType,
                OrganiserID = userId.Value
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                Message = "Event created successfully.",
                newEvent.EventID,
                newEvent.EventName,
                newEvent.Description,
                newEvent.EventDate,
                newEvent.Location,
                newEvent.Distance,
                newEvent.EventType,
                newEvent.OrganiserID
            });
        }

        // ============================================
        // PUT: api/Events/2
        // Update an existing event
        // ============================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromBody] CreateEventDto request)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Organiser")
            {
                return StatusCode(
                    403,
                    "Only organisers can update events."
                );
            }

            var existingEvent = await _context.Events
                .FindAsync(id);

            if (existingEvent == null)
            {
                return NotFound("Event not found.");
            }

            // Only the organiser who created the event
            // can update it
            if (existingEvent.OrganiserID != userId.Value)
            {
                return StatusCode(
                    403,
                    "You can only update your own events."
                );
            }

            if (request.EventDate <= DateTime.Now)
            {
                return BadRequest(
                    "Event date must be in the future."
                );
            }

            if (!IsValidEventType(request.EventType))
            {
                return BadRequest(
                    "EventType must be one of: Run, Walk, Cycle."
                );
            }

            existingEvent.EventName = request.EventName;
            existingEvent.Description = request.Description;
            existingEvent.EventDate = request.EventDate;
            existingEvent.Location = request.Location;
            existingEvent.Distance = request.Distance;
            existingEvent.EventType = request.EventType;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Event updated successfully.",
                existingEvent.EventID,
                existingEvent.EventName,
                existingEvent.Description,
                existingEvent.EventDate,
                existingEvent.Location,
                existingEvent.Distance,
                existingEvent.EventType,
                existingEvent.OrganiserID
            });
        }

        // ============================================
        // DELETE: api/Events/2
        // Delete an event
        // ============================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Organiser")
            {
                return StatusCode(
                    403,
                    "Only organisers can delete events."
                );
            }

            var existingEvent = await _context.Events
                .FindAsync(id);

            if (existingEvent == null)
            {
                return NotFound("Event not found.");
            }

            // Only the event owner can delete it
            if (existingEvent.OrganiserID != userId.Value)
            {
                return StatusCode(
                    403,
                    "You can only delete your own events."
                );
            }

            _context.Events.Remove(existingEvent);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Event deleted successfully.",
                EventID = id
            });
        }

        // ============================================
        // Helper: Validate event type
        // ============================================
        private static bool IsValidEventType(string eventType)
        {
            return eventType == "Run"
                || eventType == "Walk"
                || eventType == "Cycle";
        }
    }

    // ============================================
    // DTO: Data required to create/update an event
    // ============================================
    public class CreateEventDto
    {
        [Required]
        [MaxLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Distance { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string EventType { get; set; } = string.Empty;
    }
}