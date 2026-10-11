using Xunit;
using RaceDayAPI.Models;

namespace RaceDayAPI.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void User_ShouldHaveParticipantRole()
        {
            // Arrange
            var user = new User
            {
                Role = "Participant"
            };

            // Act
            string actualRole = user.Role;

            // Assert
            Assert.Equal("Participant", actualRole);
        }
    }
}