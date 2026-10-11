using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // ============================================
        // GET: api/Categories
        // View all categories
        // ============================================
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Select(c => new
                {
                    c.CategoryID,
                    c.EventID,
                    c.CategoryName,
                    c.CategoryType
                })
                .ToListAsync();

            return Ok(categories);
        }

        // ============================================
        // POST: api/Categories
        // Organisers create categories
        // ============================================
        [HttpPost]
        public async Task<IActionResult> CreateCategory(
            [FromBody] CategoryDto request)
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
                    "Only organisers can create categories."
                );
            }

            var eventRecord = await _context.Events
                .FindAsync(request.EventID);

            if (eventRecord == null)
            {
                return NotFound("Event not found.");
            }

            if (eventRecord.OrganiserID != userId.Value)
            {
                return StatusCode(
                    403,
                    "You can only create categories for your own events."
                );
            }

            var newCategory = new Category
            {
                EventID = request.EventID,
                CategoryName = request.CategoryName,
                CategoryType = request.CategoryType
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                Message = "Category created successfully.",
                newCategory.CategoryID,
                newCategory.EventID,
                newCategory.CategoryName,
                newCategory.CategoryType
            });
        }

        // ============================================
        // PUT: api/Categories/1
        // Update a category
        // ============================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            [FromBody] CategoryDto request)
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
                    "Only organisers can update categories."
                );
            }

            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            if (category.Event == null ||
                category.Event.OrganiserID != userId.Value)
            {
                return StatusCode(
                    403,
                    "You can only update categories for your own events."
                );
            }

            // Verify the requested event exists and belongs
            // to the logged-in organiser
            var targetEvent = await _context.Events
                .FindAsync(request.EventID);

            if (targetEvent == null)
            {
                return NotFound("Target event not found.");
            }

            if (targetEvent.OrganiserID != userId.Value)
            {
                return StatusCode(
                    403,
                    "You can only move categories to your own events."
                );
            }

            category.EventID = request.EventID;
            category.CategoryName = request.CategoryName;
            category.CategoryType = request.CategoryType;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Category updated successfully.",
                category.CategoryID,
                category.EventID,
                category.CategoryName,
                category.CategoryType
            });
        }

        // ============================================
        // DELETE: api/Categories/1
        // Delete a category
        // ============================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
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
                    "Only organisers can delete categories."
                );
            }

            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            if (category.Event == null ||
                category.Event.OrganiserID != userId.Value)
            {
                return StatusCode(
                    403,
                    "You can only delete categories for your own events."
                );
            }

            // Avoid deleting categories that already
            // have participant enrolments
            bool hasEnrolments = await _context.Enrolments
                .AnyAsync(e => e.CategoryID == id);

            if (hasEnrolments)
            {
                return Conflict(
                    "Cannot delete a category with existing enrolments."
                );
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Category deleted successfully.",
                CategoryID = id
            });
        }
    }

    // ============================================
    // DTO: Data required for creating/updating
    // ============================================
    public class CategoryDto
    {
        [Required]
        public int EventID { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string CategoryType { get; set; } = string.Empty;
    }
}