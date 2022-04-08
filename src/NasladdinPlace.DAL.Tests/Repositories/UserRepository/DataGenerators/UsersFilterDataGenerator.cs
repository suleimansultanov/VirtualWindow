using System;
using System.Collections;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Users.Search.Model;

namespace NasladdinPlace.DAL.Tests.Repositories.UserRepository.DataGenerators
{
    public class UsersFilterDataGenerator : IEnumerable
    {
        private const string DefaultUserName = "Test User";
        private const string DefaultEmail = "test@gmail.com";
        private const bool DefaultPhoneNumberConfirmed = true;
        private const string DefaultPhoneNumber = "79990000001";

        public IEnumerator GetEnumerator()
        {
            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    UserName = "Test User",
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                1
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed),
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed),
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                3
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    Email = "test@gmail.com",
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                1
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, false),
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, false)
                },
                new Filter
                {
                    PhoneNumberConfirmed = true,
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                0
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, false)
                },
                new Filter
                {
                    PhoneNumberConfirmed = false,
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                1
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    PhoneNumber = "79990000001",
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                1
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed),
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed),
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    RegistrationInitiationDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.MinValue,
                        Until = DateTime.MaxValue
                    },
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                3
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    RegistrationInitiationDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(1),
                        Until = DateTime.UtcNow.AddDays(2)
                    },
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                0
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    UserName = DefaultUserName,
                    Email = DefaultEmail,
                    PhoneNumber = DefaultPhoneNumber,
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                1
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    UserName = "Test User1",
                    Email = "test@gmail.com",
                    PhoneNumber = "79990000001",
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                0
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(-2),
                        Until = DateTime.UtcNow.AddDays(1)
                    },
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                1
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(1),
                        Until = DateTime.UtcNow.AddDays(2)
                    },
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                0
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(-2),
                        Until = DateTime.UtcNow.AddDays(1)
                    },
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                1
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(1),
                        Until = DateTime.UtcNow.AddDays(2)
                    },
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                0
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(1),
                        Until = DateTime.UtcNow.AddDays(2)
                    },
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                0
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(-2),
                        Until = DateTime.UtcNow.AddDays(1)
                    },
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                    SortBy = "Id"
                },
                1
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(-2),
                        Until = DateTime.UtcNow.AddDays(1)
                    },
                    SortBy = "Id"
                },
                1
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(1),
                        Until = DateTime.UtcNow.AddDays(2)
                    },
                    SortBy = "Id"
                },
                0
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    RegistrationInitiationDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(-2),
                        Until = DateTime.UtcNow.AddDays(1)
                    },
                    RegistrationCompletionDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(-2),
                        Until = DateTime.UtcNow.AddDays(1)
                    },
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(-2),
                        Until = DateTime.UtcNow.AddDays(1)
                    },
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(1),
                        Until = DateTime.UtcNow.AddDays(2)
                    },
                    SortBy = "Id"
                },
                0
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    Email = "test@gmail.com",
                    RegistrationInitiationDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(-2),
                        Until = DateTime.UtcNow.AddDays(1)
                    },
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(-2),
                        Until = DateTime.UtcNow.AddDays(1)
                    },
                    SortBy = "Id"
                },
                1
            };

            yield return new object[]
            {
                new Collection<ApplicationUser>
                {
                    GenerateUserWithInitiationAndCompletionDateTime(DefaultUserName, DefaultEmail, DefaultPhoneNumber, DefaultPhoneNumberConfirmed)
                },
                new Filter
                {
                    Email = "test@gmail1.com",
                    RegistrationInitiationDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(-2),
                        Until = DateTime.UtcNow.AddDays(1)
                    },
                    RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                    PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange
                    {
                        From = DateTime.UtcNow.AddDays(-2),
                        Until = DateTime.UtcNow.AddDays(1)
                    },
                    SortBy = "Id"
                },
                0
            };
        }

        public static ApplicationUser GenerateUserWithInitiationAndCompletionDateTime(
            string userName, 
            string email,
            string phoneNumber, 
            bool phoneNumberConfirmed)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                PhoneNumberConfirmed = phoneNumberConfirmed,
                PhoneNumber = phoneNumber
            };

            user.NotifyRegistrationCompletion();
            user.NotifyBankingCardVerificationInitiation();
            user.NotifyPaymentCardConfirmationCompletion();

            return user;
        }
    }
}
