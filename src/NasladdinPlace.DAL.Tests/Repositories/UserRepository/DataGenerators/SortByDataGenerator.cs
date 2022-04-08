using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.Tests.Repositories.UserRepository.DataGenerators
{
    public class SortByDataGenerator : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new object[]
            {
                nameof(ApplicationUser.UserName),
                GenerateOrderByExpectedPredicate(user => user.UserName)
            };

            yield return new object[]
            {
                nameof(ApplicationUser.Email),
                GenerateOrderByExpectedPredicate(user => user.Email)
            };

            yield return new object[]
            {
                nameof(ApplicationUser.PhoneNumber),
                GenerateOrderByExpectedPredicate(user => user.PhoneNumber)
            };

            yield return new object[]
            {
                nameof(ApplicationUser.PhoneNumberConfirmed),
                GenerateOrderByExpectedPredicate(user => user.PhoneNumberConfirmed)
            };

            yield return new object[]
            {
                nameof(ApplicationUser.RegistrationInitiationDate),
                GenerateOrderByExpectedPredicate(user => user.RegistrationInitiationDate)
            };

            yield return new object[]
            {
                nameof(ApplicationUser.RegistrationCompletionDate),
                GenerateOrderByExpectedPredicate(user => user.RegistrationCompletionDate)
            };

            yield return new object[]
            {
                nameof(ApplicationUser.PaymentCardVerificationInitiationDate),
                GenerateOrderByExpectedPredicate(user => user.PaymentCardVerificationInitiationDate)
            };

            yield return new object[]
            {
                nameof(ApplicationUser.PaymentCardVerificationCompletionDate),
                GenerateOrderByExpectedPredicate(user => user.PaymentCardVerificationCompletionDate)
            };
        }

        private static Expression<Func<ApplicationUser, T>> GenerateOrderByExpectedPredicate<T>(Expression<Func<ApplicationUser, T>> predicate)
        {
            return predicate;
        }
    }
}
