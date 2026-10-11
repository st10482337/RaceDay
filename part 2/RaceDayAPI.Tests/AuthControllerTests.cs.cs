using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Controllers;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using Xunit;

namespace RaceDayAPI.Tests
{
    public class AuthControllerTests
    {
        private RaceDayDbContext CreateTestDatabase()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new RaceDayDbContext(options);
        }

        [Fact]
        public async Task Register_ValidParticipant_Returns201()
        {
            using var context = CreateTestDatabase();
            var controller = new AuthController(context);

            var request = new RegisterDto
            {
                FirstName = "Lerato",
                LastName = "Jele",
                Email = "lerato@test.com",
                Password = "Test123!",
                Role = "Participant"
            };

            var result = await controller.Register(request);

            var response = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, response.StatusCode);

            Assert.Equal(1, await context.Users.CountAsync());
        }

        [Fact]
        public async Task Register_InvalidRole_ReturnsBadRequest()
        {
            using var context = CreateTestDatabase();
            var controller = new AuthController(context);

            var request = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.com",
                Password = "Test123!",
                Role = "Admin"
            };

            var result = await controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsConflict()
        {
            using var context = CreateTestDatabase();
            var controller = new AuthController(context);

            var request = new RegisterDto
            {
                FirstName = "Lerato",
                LastName = "Jele",
                Email = "duplicate@test.com",
                Password = "Test123!",
                Role = "Participant"
            };

            await controller.Register(request);

            var result = await controller.Register(request);

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Login_UnknownEmail_ReturnsUnauthorized()
        {
            using var context = CreateTestDatabase();
            var controller = new AuthController(context);

            var request = new LoginDto
            {
                Email = "unknown@test.com",
                Password = "Wrong123!"
            };

            var result = await controller.Login(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_WrongPassword_ReturnsUnauthorized()
        {
            using var context = CreateTestDatabase();
            var controller = new AuthController(context);

            await controller.Register(new RegisterDto
            {
                FirstName = "Thabo",
                LastName = "Mokoena",
                Email = "thabo@test.com",
                Password = "Correct123!",
                Role = "Organiser"
            });

            var result = await controller.Login(new LoginDto
            {
                Email = "thabo@test.com",
                Password = "Wrong123!"
            });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    

    [Fact]
        public async Task Login_ValidCredentials_ReturnsOkAndStoresSession()
        {
            using var context = CreateTestDatabase();

            var controller = new AuthController(context);

            // Register a test participant
            await controller.Register(new RegisterDto
            {
                FirstName = "Lerato",
                LastName = "Jele",
                Email = "lerato.login@test.com",
                Password = "Correct123!",
                Role = "Participant"
            });

            // Create a simulated HTTP session
            var session = new TestSession();

            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set<ISessionFeature>(
                new SessionFeature { Session = session });

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Attempt login
            var result = await controller.Login(new LoginDto
            {
                Email = "lerato.login@test.com",
                Password = "Correct123!"
            });

            // Verify login succeeded
            Assert.IsType<OkObjectResult>(result);

            // Verify session contains the correct role
            Assert.Equal("Participant", session.GetString("Role"));

            // Verify the user's ID was stored
            Assert.True(session.GetInt32("UserID") > 0);
        }
    }

}