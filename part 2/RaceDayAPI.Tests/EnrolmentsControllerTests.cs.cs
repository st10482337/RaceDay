using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Controllers;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using Xunit;

namespace RaceDayAPI.Tests
{
    public class EnrolmentsControllerTests
    {
        // Create a separate database for each test
        private RaceDayDbContext CreateTestDatabase()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new RaceDayDbContext(options);
        }

        // Simulate a logged-in user
        private EnrolmentsController CreateController(
            RaceDayDbContext context,
            int? userId,
            string? role)
        {
            var controller = new EnrolmentsController(context);
            var session = new TestSession();

            if (userId.HasValue)
            {
                session.SetInt32("UserID", userId.Value);
            }

            if (role != null)
            {
                session.SetString("Role", role);
            }

            var httpContext = new DefaultHttpContext();

            httpContext.Features.Set<ISessionFeature>(
                new SessionFeature { Session = session });

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controller;
        }

        // Create a test event and its category
        private async Task<(int EventId, int CategoryId)> SeedEventAsync(
            RaceDayDbContext context,
            bool pastEvent = false)
        {
            var race = new Event
            {
                EventName = "Johannesburg Marathon",
                Description = "Annual running event",
                EventDate = pastEvent
                    ? DateTime.Now.AddDays(-10)
                    : DateTime.Now.AddDays(30),
                Location = "Johannesburg",
                Distance = "42km",
                EventType = "Run",
                OrganiserID = 1
            };

            context.Events.Add(race);
            await context.SaveChangesAsync();

            var category = new Category
            {
                CategoryName = "Full Marathon",
                EventID = race.EventID
            };

            context.Categories.Add(category);
            await context.SaveChangesAsync();

            return (race.EventID, category.CategoryID);
        }

        // TEST 1: Participant successfully enrols
        [Fact]
        public async Task CreateEnrolment_Participant_Returns201()
        {
            using var context = CreateTestDatabase();

            var (eventId, categoryId) =
                await SeedEventAsync(context);

            var controller = CreateController(
                context, 2, "Participant");

            var result = await controller.CreateEnrolment(
                new CreateEnrolmentDto
                {
                    EventID = eventId,
                    CategoryID = categoryId
                });

            var response = Assert.IsType<ObjectResult>(result);

            Assert.Equal(201, response.StatusCode);

            var saved = await context.Enrolments.SingleAsync();

            Assert.Equal(2, saved.ParticipantID);
            Assert.Equal(eventId, saved.EventID);
            Assert.Equal(categoryId, saved.CategoryID);
            Assert.Equal("Enrolled", saved.Status);
        }

        // TEST 2: Organiser cannot enrol
        [Fact]
        public async Task CreateEnrolment_Organiser_Returns403()
        {
            using var context = CreateTestDatabase();

            var controller = CreateController(
                context, 1, "Organiser");

            var result = await controller.CreateEnrolment(
                new CreateEnrolmentDto
                {
                    EventID = 1,
                    CategoryID = 1
                });

            var response = Assert.IsType<ObjectResult>(result);

            Assert.Equal(403, response.StatusCode);
        }

        // TEST 3: User must be logged in
        [Fact]
        public async Task CreateEnrolment_NotLoggedIn_Returns401()
        {
            using var context = CreateTestDatabase();

            var controller = CreateController(
                context, null, null);

            var result = await controller.CreateEnrolment(
                new CreateEnrolmentDto
                {
                    EventID = 1,
                    CategoryID = 1
                });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // TEST 4: Past events cannot be entered
        [Fact]
        public async Task CreateEnrolment_PastEvent_ReturnsBadRequest()
        {
            using var context = CreateTestDatabase();

            var (eventId, categoryId) =
                await SeedEventAsync(context, true);

            var controller = CreateController(
                context, 2, "Participant");

            var result = await controller.CreateEnrolment(
                new CreateEnrolmentDto
                {
                    EventID = eventId,
                    CategoryID = categoryId
                });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 5: Duplicate enrolments are rejected
        [Fact]
        public async Task CreateEnrolment_Duplicate_ReturnsConflict()
        {
            using var context = CreateTestDatabase();

            var (eventId, categoryId) =
                await SeedEventAsync(context);

            var controller = CreateController(
                context, 2, "Participant");

            var request = new CreateEnrolmentDto
            {
                EventID = eventId,
                CategoryID = categoryId
            };

            await controller.CreateEnrolment(request);

            var result = await controller.CreateEnrolment(request);

            Assert.IsType<ConflictObjectResult>(result);

            Assert.Equal(1, await context.Enrolments.CountAsync());
        }

        // TEST 6: Category must belong to selected event
        [Fact]
        public async Task CreateEnrolment_WrongCategory_ReturnsBadRequest()
        {
            using var context = CreateTestDatabase();

            var (eventId, _) = await SeedEventAsync(context);
            var (_, otherCategoryId) = await SeedEventAsync(context);

            var controller = CreateController(
                context, 2, "Participant");

            var result = await controller.CreateEnrolment(
                new CreateEnrolmentDto
                {
                    EventID = eventId,
                    CategoryID = otherCategoryId
                });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 7: Participant can cancel own enrolment
        [Fact]
        public async Task CancelEnrolment_Owner_ReturnsOk()
        {
            using var context = CreateTestDatabase();

            var (eventId, categoryId) =
                await SeedEventAsync(context);

            var controller = CreateController(
                context, 2, "Participant");

            await controller.CreateEnrolment(
                new CreateEnrolmentDto
                {
                    EventID = eventId,
                    CategoryID = categoryId
                });

            var enrolmentId = await context.Enrolments
                .Select(e => e.EnrolmentID)
                .SingleAsync();

            var result = await controller.CancelEnrolment(enrolmentId);

            Assert.IsType<OkObjectResult>(result);

            Assert.Equal(0, await context.Enrolments.CountAsync());
        }

        // TEST 8: Participant cannot cancel someone else's enrolment
        [Fact]
        public async Task CancelEnrolment_NotOwner_Returns403()
        {
            using var context = CreateTestDatabase();

            var (eventId, categoryId) =
                await SeedEventAsync(context);

            var ownerController = CreateController(
                context, 2, "Participant");

            await ownerController.CreateEnrolment(
                new CreateEnrolmentDto
                {
                    EventID = eventId,
                    CategoryID = categoryId
                });

            var enrolmentId = await context.Enrolments
                .Select(e => e.EnrolmentID)
                .SingleAsync();

            var otherController = CreateController(
                context, 3, "Participant");

            var result = await otherController.CancelEnrolment(
                enrolmentId);

            var response = Assert.IsType<ObjectResult>(result);

            Assert.Equal(403, response.StatusCode);

            Assert.Equal(1, await context.Enrolments.CountAsync());
        }
    }
}
