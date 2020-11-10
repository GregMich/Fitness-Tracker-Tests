using System;
using System.Collections.Generic;
using System.Text;
using Fitness_Tracker.Data.Entities;
using Fitness_Tracker.Data.Contexts;
using Fitness_Tracker.Infrastructure.PasswordSecurity;
using Fitness_Tracker.Data.DataTransferObjects;

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

        public static void SeedTestDatabaseUsersWithDailyNutritionLogs(this ApplicationDbContext context)
        {
            var seededData = new List<User>
            {
                new User
                {
                    UserId = 1,
                    FirstName = "Greg",
                    LastName = "Michael",
                    Username = "gmichael",
                    Email = "gmichael@gmail.com",
                    BirthDate = new DateTime(1995, 9, 15),
                    PasswordHash = PasswordSecurity.HashPassword("secret"),
                    DailyNutritionLogs = new List<DailyNutritionLog>
                    {
                        new DailyNutritionLog
                        {
                            NutritionLogDate = new DateTime(2020, 9, 20),
                            FoodEntries = new List<FoodEntry>
                            {
                                new FoodEntry
                                {
                                    Calories = 300
                                },
                                new FoodEntry
                                {
                                    Calories = 500
                                },
                                new FoodEntry
                                {
                                    Calories = 1200
                                }
                            }
                        },
                        new DailyNutritionLog
                        {
                            NutritionLogDate = new DateTime(2020, 10, 15),
                            FoodEntries = new List<FoodEntry>
                            {
                                new FoodEntry
                                {
                                    Calories = 400
                                },
                                new FoodEntry
                                {
                                    Calories = 871
                                },
                                new FoodEntry
                                {
                                    Calories = 924
                                },
                                new FoodEntry
                                {
                                    Calories = 127
                                }
                            }
                        },
                        new DailyNutritionLog
                        {
                            NutritionLogDate = new DateTime(2020, 9, 27),
                            FoodEntries = new List<FoodEntry>
                            {
                                new FoodEntry
                                {
                                    Calories = 800
                                },
                                new FoodEntry
                                {
                                    Calories = 378
                                }
                            }
                        }
                    }
                },
                // TODO add other nutrition log data to other users
                new User
                {
                    UserId = 2,
                    FirstName = "John",
                    LastName = "Moeller",
                    Username = "jmoeller",
                    Email = "jmoeller@test.com",
                    BirthDate = new DateTime(1990, 12, 23),
                    PasswordHash = PasswordSecurity.HashPassword("secret2"),
                    DailyNutritionLogs = new List<DailyNutritionLog>
                    {
                        new DailyNutritionLog
                        {
                            NutritionLogDate = new DateTime(2020, 9, 20),
                            FoodEntries = new List<FoodEntry>
                            {
                                new FoodEntry
                                {
                                    Calories = 300
                                },
                                new FoodEntry
                                {
                                    Calories = 500
                                },
                                new FoodEntry
                                {
                                    Calories = 1200
                                }
                            }
                        },
                        new DailyNutritionLog
                        {
                            NutritionLogDate = new DateTime(2020, 10, 15),
                            FoodEntries = new List<FoodEntry>
                            {
                                new FoodEntry
                                {
                                    Calories = 400
                                },
                                new FoodEntry
                                {
                                    Calories = 871
                                },
                                new FoodEntry
                                {
                                    Calories = 924
                                },
                                new FoodEntry
                                {
                                    Calories = 127
                                }
                            }
                        }
                    }
                },
                new User
                {
                    FirstName = "Marcus",
                    LastName = "Fenix",
                    Username = "mfenix",
                    Email = "mfenix@test.com",
                    BirthDate = new DateTime(1999, 12, 12),
                    PasswordHash = PasswordSecurity.HashPassword("secret3"),
                    DailyNutritionLogs = new List<DailyNutritionLog>()
                }
            };

            context.AddRange(seededData);
            context.SaveChanges();
        }
    }
}
