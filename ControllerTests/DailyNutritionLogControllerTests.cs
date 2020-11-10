using Fitness_Tracker.Controllers;
using Xunit;
using Moq;
using TestSupport.EfHelpers;
using Fitness_Tracker.Data.Contexts;
using System.Threading.Tasks;
using Fitness_Tracker_Tests.TestSetup;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Fitness_Tracker.Data.DataTransferObjects;
using System.Collections.Generic;

namespace Fitness_Tracker_Tests.ControllerTests
{
    public class DailyNutritionLogControllerTests
    {

        [Theory]
        [InlineData(1, 3)]
        [InlineData(2, 2)]
        [InlineData(3, 0)]
        public async Task DailyNutritionLogControllerReturnsLogs(int userId, int numberOfLogsResult)
        {
            var options = SqliteInMemory
                .CreateOptions<ApplicationDbContext>();

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedTestDatabaseUsersWithDailyNutritionLogs();
            }

            using (var context = new ApplicationDbContext(options))
            {
                DailyNutritionLogController controller = new DailyNutritionLogController(
                    context,
                    TestUtilities.SetUpMockClaimsManager(userId).Object,
                    TestUtilities.GenericSetUpControllerStubLogger<DailyNutritionLogController>().Object);

                controller.ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext {  User = TestUtilities.setUpSampleHttpContextUser() }
                };

                var result = await controller.Get(userId) as OkObjectResult;
                var resultData = result.Value as List<DailyNutritionLogDTO>;

                Assert.Equal(numberOfLogsResult, resultData.Count);
                foreach (DailyNutritionLogDTO dto in resultData)
                {
                    Assert.Equal(userId, dto.UserId);
                }
            }
        }

        [Fact]
        public async Task DailyNutritionLogControllerForbidsIncorrectUser()
        {
            var options = SqliteInMemory
                .CreateOptions<ApplicationDbContext>();

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedTestDatabaseUsersWithDailyNutritionLogs();
            }

            using (var context = new ApplicationDbContext(options))
            {
                DailyNutritionLogController controller = new DailyNutritionLogController(
                    context,
                    TestUtilities.SetUpMockClaimsManager(1).Object,
                    TestUtilities.GenericSetUpControllerStubLogger<DailyNutritionLogController>().Object);

                controller.ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext { User = TestUtilities.setUpSampleHttpContextUser() }
                };

                var result = await controller.Get(2);
                Assert.IsType<ForbidResult>(result);

            }
        }
    }
}
