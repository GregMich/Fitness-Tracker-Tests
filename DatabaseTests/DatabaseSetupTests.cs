using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using System.Linq;
using Fitness_Tracker.Data.Contexts;
using Fitness_Tracker_Tests.TestSetup;

namespace Fitness_Tracker_Tests.DatabaseTests
{
    public class DatabaseSetupTests
    {

        [Fact]
        public void TestSqliteInMemoryOk()
        {
            var options = SqliteInMemory
                .CreateOptions<ApplicationDbContext>();

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedTestDatabaseUsers();

                var users = context
                    .Users
                    .ToList();

                Assert.Equal(3, users.Count());
            }
        }
    }
}
