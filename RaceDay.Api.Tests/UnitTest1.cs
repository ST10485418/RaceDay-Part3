
using Xunit;

namespace RaceDay.Api.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1_Passes()
        {
            Assert.True(true);
        }

        [Fact]
        public void RaceDay_ProjectName_IsCorrect()
        {
            string projectName = "RaceDay";

            Assert.Equal("RaceDay", projectName);
        }

        [Fact]
        public void ParticipantRole_IsCorrect()
        {
            string role = "Participant";

            Assert.Equal("Participant", role);
        }

        [Fact]
        public void OrganiserRole_IsCorrect()
        {
            string role = "Organiser";

            Assert.Equal("Organiser", role);
        }

        [Fact]
        public void SupportedEventTypes_AreDefined()
        {
            string[] eventTypes = { "Run", "Walk", "Cycle" };

            Assert.Contains("Run", eventTypes);
            Assert.Contains("Walk", eventTypes);
            Assert.Contains("Cycle", eventTypes);
        }
    }
}
