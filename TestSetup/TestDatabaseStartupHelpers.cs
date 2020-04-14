using System;
using System.Collections.Generic;
using System.Text;
using Fitness_Tracker.Data.Entities;
using Fitness_Tracker.Data.Contexts;
using Fitness_Tracker.Infrastructure.PasswordSecurity;

namespace Fitness_Tracker_Tests.TestSetup
{
    public static class TestDatabaseStartupHelpers
    {
        public static void SeedTestDatabaseUsers(this ApplicationDbContext context)
        {
            var seededUsers = new List<User>
            {
                new User
                {
                    UserId = 1,
                    FirstName = "Greg",
                    LastName = "Michael",
                    Username = "gmichael",
                    Email = "gmichael@gmail.com",
                    BirthDate = new DateTime(1995, 9, 15),
                    PasswordHash = PasswordSecurity.HashPassword("secret")
                },
                new User
                {
                    UserId = 2,
                    FirstName = "John",
                    LastName = "Moeller",
                    Username = "jmoeller",
                    Email = "jmoeller@test.com",
                    BirthDate = new DateTime(1990, 12, 23),
                    PasswordHash = PasswordSecurity.HashPassword("secret2")
                },
                new User
                {
                    FirstName = "Marcus",
                    LastName = "Fenix",
                    Username = "mfenix",
                    Email = "mfenix@test.com",
                    BirthDate = new DateTime(1999, 12, 12),
                    PasswordHash = PasswordSecurity.HashPassword("secret3")
                }
            };

            context.AddRange(seededUsers);
            context.SaveChanges();
        }
    }
}
