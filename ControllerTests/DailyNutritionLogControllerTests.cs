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
using Fitness_Tracker.Data.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace Fitness_Tracker_Tests.ControllerTests
{
    public class DailyNutritionLogControllerTests
    {

        [Theory]
        [InlineData(1, 3)]
        [InlineData(2, 2)]
        [InlineData(3, 0)]
        public async Task GET_DailyNutritionLogControllerReturnsLogs(int userId, int numberOfLogsResult)
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
        public async Task GET_DailyNutritionLogControllerForbidsIncorrectUser()
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

        [Theory]
        [InlineData(1, 3)]
        [InlineData(1, 0)]
        [InlineData(2, 5)]
        public async Task POST_DailyNutritionLogControllerActionCreatesDailyNutritionLog(int userId, int numberOfFoodEntries)
        {
            int newEntryId = 0;
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
                    HttpContext = new DefaultHttpContext { User = TestUtilities.setUpSampleHttpContextUser() }
                };

                var newDailyNutritionLog = new DailyNutritionLog
                {
                    UserId = userId,
                    NutritionLogDate = new DateTime(month:9, day:12, year:2020),
                    FoodEntries = new List<FoodEntry>()
                };

                Random rand = new Random();
                for (int i = 0; i < numberOfFoodEntries; i++)
                {
                    newDailyNutritionLog.FoodEntries.Add(new FoodEntry
                    {
                        Calories = rand.Next(300, 800)
                    });
                }

                CreatedAtActionResult result = (CreatedAtActionResult)(await controller.Post(1, newDailyNutritionLog));
                Assert.IsType<CreatedAtActionResult>(result);
                var resultData = result.Value as DailyNutritionLog;
                newEntryId = resultData.DailyNutritionLogId;
            }

            using (var context = new ApplicationDbContext(options))
            {
                var newEntry = await context
                    .DailyNutritionLogs
                    .Where(_ => _.DailyNutritionLogId == newEntryId)
                    .Include(_ => _.FoodEntries)
                    .FirstOrDefaultAsync();

                Assert.NotNull(newEntry);
                Assert.Equal(numberOfFoodEntries, newEntry.FoodEntries.Count());
                foreach (FoodEntry foodEntry in newEntry.FoodEntries)
                {
                    Assert.Equal(newEntryId, foodEntry.DailyNutritionLogId);
                }
            }
        }
    }
}
