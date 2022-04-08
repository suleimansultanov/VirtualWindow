using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Users.Manager;
using NasladdinPlace.DAL.Contracts;

namespace NasladdinPlace.Api.Tests.Utils
{
    public class FakeUserManager : IUserManager
    {
        private const int PhoneNumberChangeTokenDigitMaxValue = 9;
        private const int PhoneNumberChangeTokenDigitsNumber = 6;
        
        private readonly IApplicationDbContextFactory _dbContextFactory;
        private readonly ConcurrentDictionary<string, string> _changeTokensByPhoneNumberDictionary;
        private readonly ConcurrentDictionary<int, string> _phoneNumberResetTokensByUserIdDictionary;
        private readonly Random _random;

        public FakeUserManager(IApplicationDbContextFactory dbContextFactory)
        {
            if (dbContextFactory == null)
                throw new ArgumentNullException(nameof(dbContextFactory));
            
            _dbContextFactory = dbContextFactory;
            _changeTokensByPhoneNumberDictionary = new ConcurrentDictionary<string, string>();
            _phoneNumberResetTokensByUserIdDictionary = new ConcurrentDictionary<int, string>();
            _random = new Random();
        }
        
        public async Task<UserManagerResult> CreateAsync(ApplicationUser user)
        {
            using (var context = _dbContextFactory.Create())
            {
                if (context.Users.Any(u => u.UserName == user.UserName))
                {
                    return UserManagerResult.Failure();
                }

                user.NormalizedUserName = user.UserName;

                context.Users.Add(user);
                await context.SaveChangesAsync();
                
                return UserManagerResult.Success();
            }
        }

        public async Task<UserManagerResult> UpdateAsync(ApplicationUser user)
        {
            using (var context = _dbContextFactory.Create())
            {
                context.Users.Update(user);
                
                await context.SaveChangesAsync();
                
                return UserManagerResult.Success();
            }
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            using (var context = _dbContextFactory.Create())
            {
                return await context.Users.SingleOrDefaultAsync(u => u.UserName == userName);
            }
        }

        public Task<string> GenerateChangePhoneNumberTokenAsync(ApplicationUser user, string phoneNumber)
        {
            var token = GeneratePhoneNumberChangeToken();
            _changeTokensByPhoneNumberDictionary[phoneNumber] = token;
            return Task.FromResult(token);
        }

        public async Task<UserManagerResult> ChangePhoneNumberAsync(ApplicationUser user, string phoneNumber, string changeToken)
        {
            if (!_changeTokensByPhoneNumberDictionary.TryGetValue(phoneNumber, out var existingToken))
            {
                return UserManagerResult.Failure();
            }

            _changeTokensByPhoneNumberDictionary[phoneNumber] = null;
            
            if (string.IsNullOrWhiteSpace(existingToken) || changeToken != existingToken) 
                return UserManagerResult.Failure();

            user.PhoneNumber = phoneNumber;
            user.PhoneNumberConfirmed = true;

            using (var context = _dbContextFactory.Create())
            {
                context.Update(user);
                await context.SaveChangesAsync();
            }
            
            return UserManagerResult.Success();
        }

        public Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            var passwordResetToken = GeneratePasswordResetToken();
            _phoneNumberResetTokensByUserIdDictionary[user.Id] = passwordResetToken;
            return Task.FromResult(passwordResetToken);
        }

        public Task<UserManagerResult> ResetPasswordAsync(ApplicationUser user, string resetToken, string password)
        {
            if (!_phoneNumberResetTokensByUserIdDictionary.TryGetValue(user.Id, out var existingToken))
            {
                return Task.FromResult(UserManagerResult.Failure());
            }

            _phoneNumberResetTokensByUserIdDictionary[user.Id] = null;

            if (string.IsNullOrWhiteSpace(existingToken) || resetToken != existingToken)
                return Task.FromResult(UserManagerResult.Failure());

            return Task.FromResult(UserManagerResult.Success());
        }

        public async Task<ApplicationUser> FindByIdAsync(int userId)
        {
            using (var context = _dbContextFactory.Create())
            {
                return await context.Users.SingleOrDefaultAsync(u => u.Id == userId);
            }
        }

        public async Task<ApplicationUser> FindByIdIncludePaymentCardsAsync(int userId)
        {
            using (var context = _dbContextFactory.Create())
            {
                return await context.Users
                    .Include(u => u.PaymentCards)
                    .Include(u=>u.ActivePaymentCard)
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }
        }

        private string GeneratePhoneNumberChangeToken()
        {
            return new string(NextRandomAsChar(PhoneNumberChangeTokenDigitMaxValue), PhoneNumberChangeTokenDigitsNumber);
        }

        private string GeneratePasswordResetToken()
        {
            return Guid.NewGuid().ToString();
        }

        private char NextRandomAsChar(int maxValue)
        {
            return NextRandom(maxValue).ToString()[0];
        }

        private int NextRandom(int maxValue)
        {
            return _random.Next(maxValue);
        }
    }
}