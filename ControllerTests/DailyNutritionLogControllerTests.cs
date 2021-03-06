﻿using Fitness_Tracker.Controllers;
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

                CreatedAtActionResult result = (CreatedAtActionResult)(await controller.Post(userId, newDailyNutritionLog));
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

        [Fact]
        public async Task POST_DailyNutritionLogControllerActionReturnsBadRequestWithInvalidModel()
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
                var controller = new DailyNutritionLogController(
                    context,
                    TestUtilities.SetUpMockClaimsManager(2).Object,
                    TestUtilities.GenericSetUpControllerStubLogger<DailyNutritionLogController>().Object)
                    {
                        ControllerContext = new ControllerContext
                        {
                            HttpContext = new DefaultHttpContext {  User = TestUtilities.setUpSampleHttpContextUser() }
                        }
                    };
                // model validation will not actually occur on this object during model binding, see the following comment for more info
                var invalidDailyNutritionLog = new DailyNutritionLog
                {
                    UserId = 2,
                    FoodEntries = new List<FoodEntry>()
                };
                // model validation doesnt actually happen when we test the controller action so we have to add it manually,
                // therefore we are limited in testing the data annotations to the model binding
                controller.ModelState.AddModelError("unitTestError", "a model validation error occured");
                var result = await controller.Post(2, invalidDailyNutritionLog);
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }

        [Fact]
        public async Task POST_DailyNutritionLogControllerActionReturnsForbiddenWithWrongUser()
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
                    TestUtilities.GenericSetUpControllerStubLogger<DailyNutritionLogController>().Object)
                    {
                        ControllerContext = new ControllerContext
                        {
                            HttpContext = new DefaultHttpContext { User = TestUtilities.setUpSampleHttpContextUser() }
                        }
                    };

                var dnl = new DailyNutritionLog
                {
                    UserId = 1,
                    NutritionLogDate = new DateTime(2020, 1, 1),
                    FoodEntries = new List<FoodEntry>()
                };

                // test userId in claims manager does not match route
                var result = await controller.Post(2, dnl);
                Assert.IsType<ForbidResult>(result);

                var dnl2 = new DailyNutritionLog
                {
                    UserId = 2,
                    NutritionLogDate = new DateTime(2020, 1, 1),
                    FoodEntries = new List<FoodEntry>()
                };

                // test userId in claims manager does not match daily nutrition log
                var result_2 = await controller.Post(1, dnl2);
                Assert.IsType<ForbidResult>(result_2);

                // test both cases at the same time
                var result_3 = await controller.Post(2, dnl2);
                Assert.IsType<ForbidResult>(result_3);
            }
        }


    }
}
