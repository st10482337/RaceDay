using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Results
        // View all recorded race results
        [HttpGet]
        public async Task<IActionResult> GetResults()
        {
            var results = await _context.Results
                .AsNoTracking()
                .Select(r => new
                {
                    r.ResultID,
                    r.EnrolmentID,
                    r.FinishTime,
                    r.FinishingPosition,
                    ParticipantID = r.Enrolment != null
                        ? r.Enrolment.ParticipantID
                        : 0,
                    EventID = r.Enrolment != null
                        ? r.Enrolment.EventID
                        : 0
                })
                .ToListAsync();

            return Ok(results);
        }

        // GET: api/Results/1
        // View a specific race result
        [HttpGet("{id}")]
        public async Task<IActionResult> GetResultById(int id)
        {
            var result = await _context.Results
                .AsNoTracking()
                .Where(r => r.ResultID == id)
                .Select(r => new
                {
                    r.ResultID,
                    r.EnrolmentID,
                    r.FinishTime,
                    r.FinishingPosition,
                    ParticipantID = r.Enrolment != null
                        ? r.Enrolment.ParticipantID
                        : 0,
                    EventID = r.Enrolment != null
                        ? r.Enrolment.EventID
                        : 0
                })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound("Result not found.");
            }

            return Ok(result);
        }

        // POST: api/Results
        // Only the organiser of the event can record results
        [HttpPost]
        public async Task<IActionResult> CreateResult(
            [FromBody] CreateResultDto request)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Organiser")
            {
                return StatusCode(403,
                    "Only organisers can record race results.");
            }

            if (request.FinishTime <= TimeSpan.Zero)
            {
                return BadRequest(
                    "Finish time must be greater than zero.");
            }

            if (request.FinishingPosition <= 0)
            {
                return BadRequest(
                    "Finishing position must be greater than zero.");
            }

            var enrolment = await _context.Enrolments
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e =>
                    e.EnrolmentID == request.EnrolmentID);

            if (enrolment == null)
            {
                return NotFound("Enrolment not found.");
            }

            if (enrolment.Event == null ||
                enrolment.Event.OrganiserID != userId.Value)
            {
                return StatusCode(403,
                    "You can only record results for your own events.");
            }

            bool resultExists = await _context.Results
                .AnyAsync(r =>
                    r.EnrolmentID == request.EnrolmentID);

            if (resultExists)
            {
                return Conflict(
                    "A result already exists for this enrolment.");
            }

            var result = new Result
            {
                EnrolmentID = request.EnrolmentID,
                FinishTime = request.FinishTime,
                FinishingPosition = request.FinishingPosition
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                Message = "Race result recorded successfully.",
                result.ResultID,
                result.EnrolmentID,
                result.FinishTime,
                result.FinishingPosition
            });
        }

        // PUT: api/Results/1
        // Update an existing race result
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResult(
            int id,
            [FromBody] CreateResultDto request)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Organiser")
            {
                return StatusCode(403,
                    "Only organisers can update race results.");
            }

            if (request.FinishTime <= TimeSpan.Zero)
            {
                return BadRequest(
                    "Finish time must be greater than zero.");
            }

            if (request.FinishingPosition <= 0)
            {
                return BadRequest(
                    "Finishing position must be greater than zero.");
            }

            var result = await _context.Results
                .Include(r => r.Enrolment)
                .ThenInclude(e => e!.Event)
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (result == null)
            {
                return NotFound("Result not found.");
            }

            if (result.Enrolment?.Event == null ||
                result.Enrolment.Event.OrganiserID != userId.Value)
            {
                return StatusCode(403,
                    "You can only update results for your own events.");
            }

            if (request.EnrolmentID != result.EnrolmentID)
            {
                return BadRequest(
                    "The enrolment ID cannot be changed.");
            }

            result.FinishTime = request.FinishTime;
            result.FinishingPosition = request.FinishingPosition;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Race result updated successfully.",
                result.ResultID,
                result.EnrolmentID,
                result.FinishTime,
                result.FinishingPosition
            });
        }

        // DELETE: api/Results/1
        // Delete an existing race result
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Organiser")
            {
                return StatusCode(403,
                    "Only organisers can delete race results.");
            }

            var result = await _context.Results
                .Include(r => r.Enrolment)
                .ThenInclude(e => e!.Event)
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (result == null)
            {
                return NotFound("Result not found.");
            }

            if (result.Enrolment?.Event == null ||
                result.Enrolment.Event.OrganiserID != userId.Value)
            {
                return StatusCode(403,
                    "You can only delete results for your own events.");
            }

            _context.Results.Remove(result);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Race result deleted successfully.",
                ResultID = id
            });
        }
    }

    public class CreateResultDto
    {
        [Required]
        public int EnrolmentID { get; set; }

        [Required]
        public TimeSpan FinishTime { get; set; }

        [Required]
        public int FinishingPosition { get; set; }
    }
}