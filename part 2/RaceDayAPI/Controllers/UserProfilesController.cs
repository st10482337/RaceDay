using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public UserProfilesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/UserProfiles
        // Get the logged-in user's profile
        [HttpGet]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            var profile = await _context.UserProfiles
                .AsNoTracking()
                .Where(p => p.UserID == userId.Value)
                .Select(p => new
                {
                    p.ProfileID,
                    p.UserID,
                    p.PhoneNumber,
                    p.Address,
                    p.ProfilePictureURL
                })
                .FirstOrDefaultAsync();

            if (profile == null)
            {
                return NotFound(
                    "Profile not found. Please create your profile.");
            }

            return Ok(profile);
        }

        // POST: api/UserProfiles
        // Create a profile for the logged-in user
        [HttpPost]
        public async Task<IActionResult> CreateProfile(
            [FromBody] UserProfileDto request)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            bool profileExists = await _context.UserProfiles
                .AnyAsync(p => p.UserID == userId.Value);

            if (profileExists)
            {
                return Conflict(
                    "You already have a profile. Use PUT to update it.");
            }

            var profile = new UserProfile
            {
                UserID = userId.Value,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                ProfilePictureURL = request.ProfilePictureURL
            };

            _context.UserProfiles.Add(profile);

            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                Message = "Profile created successfully.",
                profile.ProfileID,
                profile.UserID,
                profile.PhoneNumber,
                profile.Address,
                profile.ProfilePictureURL
            });
        }

        // PUT: api/UserProfiles
        // Update the logged-in user's profile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UserProfileDto request)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserID == userId.Value);

            if (profile == null)
            {
                return NotFound(
                    "Profile not found. Please create your profile first.");
            }

            profile.PhoneNumber = request.PhoneNumber;
            profile.Address = request.Address;
            profile.ProfilePictureURL = request.ProfilePictureURL;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Profile updated successfully.",
                profile.ProfileID,
                profile.UserID,
                profile.PhoneNumber,
                profile.Address,
                profile.ProfilePictureURL
            });
        }

        // DELETE: api/UserProfiles
        // Delete the logged-in user's profile
        [HttpDelete]
        public async Task<IActionResult> DeleteProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserID == userId.Value);

            if (profile == null)
            {
                return NotFound("Profile not found.");
            }

            _context.UserProfiles.Remove(profile);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Profile deleted successfully."
            });
        }
    }

    public class UserProfileDto
    {
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(255)]
        public string? ProfilePictureURL { get; set; }
    }
}