using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class UsersDataSet: DataSet<ApplicationUser>
    {
        protected override ApplicationUser[] Data => new[]
        {
            new ApplicationUser
            {
                Id = 0,
                Email = "user1@gmail.com",
                Birthdate = DateTime.UtcNow.AddYears(-18),
                ConcurrencyStamp = "8ad13305-4d65-4ffa-903d-6f8af1966c24",
                AccessFailedCount = 0,
                EmailConfirmed = false,
                FullName = null,
                Gender = Gender.Male,
                LockoutEnabled = true,
                LockoutEnd = null,
                NormalizedEmail = "USER1@GMAIL.COM",
                NormalizedUserName = "792621030598",
                PasswordHash = "AQAAAAEAACcQAAAAEOLVZHSpr5pXKDo22pI6sf/Tr4ZUwEbXybxQSqpsBXY/zLi/R9Yd9vqFJbmbzQBSpw==",
                PhoneNumber = "79262103058",
                PhoneNumberConfirmed = true,
                SecurityStamp = "0d6576d1-8d64-418e-87c1-008f64c061d2",
                UserName = "79262103058"
            },
            new ApplicationUser
            {
                Id = 0,
                Email = "user2@domain.com",
                Birthdate = DateTime.UtcNow.AddYears(-22),
                ConcurrencyStamp = "d8cdfae2-f151-4708-83ac-8ce425532c0a",
                AccessFailedCount = 0,
                EmailConfirmed = false,
                FullName = null,
                Gender = Gender.Female,
                LockoutEnabled = true,
                LockoutEnd = null,
                NormalizedEmail = "USER2@domain.com",
                NormalizedUserName = "996777276646",
                PasswordHash = "AQAAAAEAACcQAAAAEOLVZHSpr5pXKDo22pI6sf/Tr4ZUwEbXybxQSqpsBXY/zLi/R9Yd9vqFJbmbzQBSpw==",
                PhoneNumber = "79262103059",
                PhoneNumberConfirmed = true,
                SecurityStamp = "0d6576d1-8d64-418e-87c1-008f64c061d2",
                UserName = "996777276646"                
            }
        };
    }
}