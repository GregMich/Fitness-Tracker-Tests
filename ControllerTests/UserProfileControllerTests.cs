using System;
using System.Collections.Generic;
using System.Text;
using Fitness_Tracker.Data.Entities;
using Fitness_Tracker.Controllers;
using Xunit;
using Moq;
using TestSupport.EfHelpers;
using Fitness_Tracker.Data.Contexts;
using Fitness_Tracker_Tests.TestSetup;
using Microsoft.Extensions.Logging;
using Fitness_Tracker.Infrastructure.Security;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Fitness_Tracker.Data.DataTransferObjects;

namespace Fitness_Tracker_Tests.SecurityTests
{
    public class UserProfileControllerTests
    {

        [Fact]
        public async Task UserProfileControllerReturnsUserProfile()
        {
            var options = SqliteInMemory
                .CreateOptions<ApplicationDbContext>();

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedTestDatabaseUsers();
            }

            using (var context = new ApplicationDbContext(options))
            {
                UserProfileController controller = new UserProfileController(
                    context,
                    SetUpStubLogger().Object,
                    SetUpMockClaimsManager().Object);

                controller.ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = setUpSampleHttpContextUser() }
                };

                var result = await controller.GetUserProfile() as OkObjectResult;
                var resultData = result.Value as UserProfileDTO;

                Assert.Equal(1, resultData.UserId);
                Assert.Equal("Greg", resultData.FirstName);
                Assert.Equal("Michael", resultData.LastName);
                Assert.Equal("gmichael", resultData.Username);
                Assert.Equal("gmichael@gmail.com", resultData.Email);
            }
        }

        /*
         * 
         *  Private static methods for assisting in test setup
         * 
         */

        private static Mock<ILogger<UserProfileController>> SetUpStubLogger() =>
            new Mock<ILogger<UserProfileController>>();

        private static JsonSerializerOptions getTestJsonSerializerOptions() =>
            new JsonSerializerOptions { WriteIndented = true };

        // https://stackoverflow.com/questions/38557942/mocking-iprincipal-in-asp-net-core
        private static ClaimsPrincipal setUpSampleHttpContextUser()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "example name"),
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim("custom-claim", "example claim value"),
                }, 
                "mock"));
        }

        private static Mock<IClaimsManager> SetUpMockClaimsManager()
        {
            var mock = new Mock<IClaimsManager>();
            mock.Setup(m => m.GetUserIdClaim()).Returns(1);
            return mock;
        }
    }
}
