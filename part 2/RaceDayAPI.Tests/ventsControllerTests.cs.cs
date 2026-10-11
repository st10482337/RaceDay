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
    public class EventsControllerTests
    {
        // Create a separate temporary database for each test
        private RaceDayDbContext CreateTestDatabase()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new RaceDayDbContext(options);
        }

        // Simulate a logged-in user
        private EventsController CreateController(
            RaceDayDbContext context,
            int? userId,
            string? role)
        {
            var controller = new EventsController(context);

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

        // Create valid event details
        private CreateEventDto ValidEvent()
        {
            return new CreateEventDto
            {
                EventName = "Johannesburg Marathon",
                Description = "Annual running event",
                EventDate = DateTime.Now.AddDays(30),
                Location = "Johannesburg",
                Distance = "42km",
                EventType = "Run"
            };
        }

        // TEST 1: Organiser creates an event
        [Fact]
        public async Task CreateEvent_Organiser_Returns201()
        {
            using var context = CreateTestDatabase();

            var controller = CreateController(
                context, 1, "Organiser");

            var result = await controller.CreateEvent(ValidEvent());

            var response = Assert.IsType<ObjectResult>(result);

            Assert.Equal(201, response.StatusCode);

            Assert.Equal(1, await context.Events.CountAsync());
        }

        // TEST 2: Participant cannot create an event
        [Fact]
        public async Task CreateEvent_Participant_Returns403()
        {
            using var context = CreateTestDatabase();

            var controller = CreateController(
                context, 2, "Participant");

            var result = await controller.CreateEvent(ValidEvent());

            var response = Assert.IsType<ObjectResult>(result);

            Assert.Equal(403, response.StatusCode);

            Assert.Equal(0, await context.Events.CountAsync());
        }

        // TEST 3: Unauthenticated user cannot create event
        [Fact]
        public async Task CreateEvent_NotLoggedIn_Returns401()
        {
            using var context = CreateTestDatabase();

            var controller = CreateController(
                context, null, null);

            var result = await controller.CreateEvent(ValidEvent());

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // TEST 4: Past event dates are rejected
        [Fact]
        public async Task CreateEvent_PastDate_ReturnsBadRequest()
        {
            using var context = CreateTestDatabase();

            var controller = CreateController(
                context, 1, "Organiser");

            var request = ValidEvent();

            request.EventDate = DateTime.Now.AddDays(-5);

            var result = await controller.CreateEvent(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 5: Invalid event types are rejected
        [Fact]
        public async Task CreateEvent_InvalidType_ReturnsBadRequest()
        {
            using var context = CreateTestDatabase();

            var controller = CreateController(
                context, 1, "Organiser");

            var request = ValidEvent();

            request.EventType = "Swimming";

            var result = await controller.CreateEvent(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 6: Organiser updates own event
        [Fact]
        public async Task UpdateEvent_Owner_ReturnsOk()
        {
            using var context = CreateTestDatabase();

            context.Events.Add(new Event
            {
                EventName = "Old Race",
                Description = "Old description",
                EventDate = DateTime.Now.AddDays(20),
                Location = "Pretoria",
                Distance = "10km",
                EventType = "Run",
                OrganiserID = 1
            });

            await context.SaveChangesAsync();

            var eventId = await context.Events
                .Select(e => e.EventID)
                .FirstAsync();

            var controller = CreateController(
                context, 1, "Organiser");

            var request = ValidEvent();

            request.EventName = "Updated Race";

            var result = await controller.UpdateEvent(
                eventId, request);

            Assert.IsType<OkObjectResult>(result);

            var updatedEvent = await context.Events.FindAsync(eventId);

            Assert.Equal("Updated Race", updatedEvent!.EventName);
        }

        // TEST 7: Organiser cannot update another organiser's event
        [Fact]
        public async Task UpdateEvent_NotOwner_Returns403()
        {
            using var context = CreateTestDatabase();

            context.Events.Add(new Event
            {
                EventName = "Another Race",
                Description = "Test event",
                EventDate = DateTime.Now.AddDays(20),
                Location = "Durban",
                Distance = "21km",
                EventType = "Run",
                OrganiserID = 5
            });

            await context.SaveChangesAsync();

            var eventId = await context.Events
                .Select(e => e.EventID)
                .FirstAsync();

            var controller = CreateController(
                context, 1, "Organiser");

            var result = await controller.UpdateEvent(
                eventId, ValidEvent());

            var response = Assert.IsType<ObjectResult>(result);

            Assert.Equal(403, response.StatusCode);
        }

        // TEST 8: Event owner can delete event
        [Fact]
        public async Task DeleteEvent_Owner_ReturnsOk()
        {
            using var context = CreateTestDatabase();

            context.Events.Add(new Event
            {
                EventName = "Delete Test Race",
                Description = "Temporary event",
                EventDate = DateTime.Now.AddDays(20),
                Location = "Cape Town",
                Distance = "5km",
                EventType = "Walk",
                OrganiserID = 1
            });

            await context.SaveChangesAsync();

            var eventId = await context.Events
                .Select(e => e.EventID)
                .FirstAsync();

            var controller = CreateController(
                context, 1, "Organiser");

            var result = await controller.DeleteEvent(eventId);

            Assert.IsType<OkObjectResult>(result);

            Assert.Equal(0, await context.Events.CountAsync());
        }
    }
}