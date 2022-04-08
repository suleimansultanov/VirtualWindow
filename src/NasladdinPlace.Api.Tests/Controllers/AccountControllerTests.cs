using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Controllers;
using NasladdinPlace.Api.Dtos.Account;
using NasladdinPlace.Api.Dtos.User;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.TestUtils;
using NUnit.Framework;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Payment.Card;
using NasladdinPlace.Core.Services.Payment.Card.Type;
using NasladdinPlace.Core.Services.Users.Manager;
using NasladdinPlace.Payment.Models;
using NasladdinPlace.TestUtils.Extensions;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.TestUtils.Utils;

namespace NasladdinPlace.Api.Tests.Controllers
{
    public class AccountControllerTests : TestsBase
    {
        private const string DefaultPhoneNumber = "79990000001";
        private const string DefaultCardToken = "Test Card";
        private const string InvalidCode = "1234";
        private const string DefaultUserEmail = "test@mail.com";

        private AccountController _accountController;
        private IUserManager _userManager;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            Mapper.Reset();
            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            var serviceProvider = TestServiceProviderFactory.Create();

            _accountController = serviceProvider.GetRequiredService<AccountController>();
            _userManager = serviceProvider.GetRequiredService<IUserManager>();
            
            Seeder.Seed(new PromotionSettingsDataSet());
        }

        [TestCase(UserRegistrationStatus.PhoneNumberConfirmed, UserRegistrationStatus.PhoneNumberConfirmed)]
        [TestCase(UserRegistrationStatus.None, UserRegistrationStatus.None)]
        [TestCase(UserRegistrationStatus.BankingCardConfirmed, UserRegistrationStatus.BankingCardConfirmed)]
        public void VerifyPhoneNumber_UserWithPredefinedRegistrationStatusIsGiven_ShouldReturnExpectedResult(
            UserRegistrationStatus currentUserRegistrationStatus, UserRegistrationStatus expectedPreviousUserRegistrationStatus)
        {
            var user = CreateUserAccordingToRegistrationStatusAsync(DefaultPhoneNumber, currentUserRegistrationStatus).GetAwaiter().GetResult();
            var code = GenerateVerificationCodeAsync(user, DefaultPhoneNumber).GetAwaiter().GetResult();

            var expectedResult = _accountController.VerifyPhoneNumberAsync(new VerifyPhoneNumberDto
            {
                Code = code,
                PhoneNumber = DefaultPhoneNumber
            }).GetAwaiter().GetResult();
            var okResult = expectedResult.Should().BeOfType<OkObjectResult>().Subject;
            var userPasswordResult = okResult.Value.Should().BeAssignableTo<UserConfirmationResponseDto>().Subject;
            userPasswordResult.UserPreviousRegistrationStatus.Should().Be(expectedPreviousUserRegistrationStatus);
        }
        
        [Test]
        public void VerifyPhoneNumber_CorrectVerificationCodeIsGiven_ShouldAddUserBonusPoints()
        {
            var user = CreateUserAccordingToRegistrationStatusAsync(DefaultPhoneNumber, UserRegistrationStatus.None).GetAwaiter().GetResult();
            var code = GenerateVerificationCodeAsync(user, DefaultPhoneNumber).GetAwaiter().GetResult();

            _accountController.VerifyPhoneNumberAsync(new VerifyPhoneNumberDto
            {
                Code = code,
                PhoneNumber = DefaultPhoneNumber
            }).Wait();
            
            user = Context.Users
                .Include(u => u.BonusPoints)
                .FirstOrDefault(u => u.UserName == DefaultPhoneNumber);
            
            user.Should().NotBeNull();
            user.TotalBonusPoints.Should().IsSameOrEqualTo(PromotionSettingsDataSet.VerifyPhoneNumberBonusAmount);

            user.BonusPoints.Should().HaveCount(1);
        }

        [Test]
        public void VerifyPhoneNumber_InvalidCode_ShouldReturnBadRequest()
        {
            CreateUserAccordingToRegistrationStatusAsync(DefaultPhoneNumber, UserRegistrationStatus.None).Wait();
            var expectedResult = _accountController.VerifyPhoneNumberAsync(new VerifyPhoneNumberDto
            {
                Code = InvalidCode,
                PhoneNumber = DefaultPhoneNumber
            }).GetAwaiter().GetResult();
            expectedResult.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public void VerifyPhoneNumber_InvalidCodeIsGiven_ShouldNotAddBonusesToUser()
        {
            CreateUserAccordingToRegistrationStatusAsync(DefaultPhoneNumber, UserRegistrationStatus.None).Wait();
            _accountController.VerifyPhoneNumberAsync(new VerifyPhoneNumberDto
            {
                Code = InvalidCode,
                PhoneNumber = DefaultPhoneNumber
            }).Wait();

            var user = Context.Users
                .Include(u => u.BonusPoints)
                .FirstOrDefault(u => u.UserName == DefaultPhoneNumber);
            
            user.Should().NotBeNull();
            user.TotalBonusPoints.Should().IsSameOrEqualTo(0);

            user.BonusPoints.Any().Should().BeFalse();
        }

        [Test]
        public void VerifyPhoneNumber_UserDoesNotExist_ShouldReturnBadRequest()
        {
            var expectedResult = _accountController.VerifyPhoneNumberAsync(new VerifyPhoneNumberDto
            {
                Code = InvalidCode,
                PhoneNumber = DefaultPhoneNumber
            }).GetAwaiter().GetResult();
            expectedResult.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void VerifyPhoneNumber_UserHasActivePaymentSystem_ShouldReturnUserHasBankingCardBeTrue()
        {
            var user = CreateUserAccordingToRegistrationStatusAsync(DefaultPhoneNumber, UserRegistrationStatus.PhoneNumberConfirmed).GetAwaiter().GetResult();
            AddPaymentSystemForUser(user).Wait();
            var code = GenerateVerificationCodeAsync(user, DefaultPhoneNumber).GetAwaiter().GetResult();

            var expectedResult = _accountController.VerifyPhoneNumberAsync(new VerifyPhoneNumberDto
            {
                Code = code,
                PhoneNumber = DefaultPhoneNumber
            }).GetAwaiter().GetResult();
            var okResult = expectedResult.Should().BeOfType<OkObjectResult>().Subject;
            var userPasswordResult = okResult.Value.Should().BeAssignableTo<UserConfirmationResponseDto>().Subject;
            userPasswordResult.UserPreviousRegistrationStatus.Should().Be(UserRegistrationStatus.BankingCardConfirmed);
        }

        [Test]
        public void GetUserAccountAsync_UserWithPaymentCardIsGiven_ShouldReturnUserDtoWithPaymentCard()
        {
            var user = CreateUserWithAvailableUserBonusPoints(0.0M);
            CreateUserActivePaymentCard(ref user);
            AuthorizeUser(user);

            var accountActionResult = _accountController.GetUserAccountAsync().GetAwaiter().GetResult();
            var accountDto = ((OkObjectResult) accountActionResult).Value as UserFullInfoDto;
            
            EnsureUserFullInfoDtoIsCorrect(accountDto, referenceUser: user);
        }
        
        [Test]
        public void GetUserAccountAsync_UserWithNoPaymentCardIsGiven_ShouldReturnUserDtoWithoutPaymentCard()
        {
            var user = CreateUserWithAvailableUserBonusPoints(0.0M);
            AuthorizeUser(user);

            var accountActionResult = _accountController.GetUserAccountAsync().GetAwaiter().GetResult();
            var accountDto = ((OkObjectResult) accountActionResult).Value as UserFullInfoDto;

            EnsureUserFullInfoDtoIsCorrect(accountDto, referenceUser: user);
        }
        
        [TestCase(100)]
        [TestCase(0)]
        public void GetUserAccountAsync_UserWithAvailableBonusPointsIsGiven_ShouldReturnUserDtoWithCorrectUserBonuses(
            decimal availableUserBonuses)
        {
            var user = CreateUserWithAvailableUserBonusPoints(availableUserBonuses);
            AuthorizeUser(user);

            var accountActionResult = _accountController.GetUserAccountAsync().GetAwaiter().GetResult();
            var accountDto = ((OkObjectResult) accountActionResult).Value as UserFullInfoDto;

            EnsureUserFullInfoDtoIsCorrect(accountDto, referenceUser: user);
        }

        [Test]
        public void GetUserAccountAsync_UserWithPaymentCardWithoutNumberIsGiven_ShouldReturnUserDtoWithoutPaymentCard()
        {
            var user = CreateUserWithAvailableUserBonusPoints(0.0M);
            CreateUserActivePaymentCard(ref user, includeNumberAndExpirationDate: false);
            AuthorizeUser(user);

            var accountActionResult = _accountController.GetUserAccountAsync().GetAwaiter().GetResult();
            var accountDto = ((OkObjectResult)accountActionResult).Value as UserFullInfoDto;

            EnsureUserFullInfoDtoIsCorrect(accountDto, referenceUser: user);
        }

        [Test]
        public void RequestPhoneNumberVerificationCodeAsync_PhoneNumberIsGiven_ShouldReturnOk()
        {
            var user = CreateUserWithAvailableUserBonusPoints(0.0M);
            AuthorizeUser(user);

            var sendVerificationCodeResult = _accountController.RequestPhoneNumberVerificationCodeAsync(new PhoneNumberDto
            {
                PhoneNumber = DefaultPhoneNumber
            }).GetAwaiter().GetResult();

            Context.UserNotifications.Should().NotBeNullOrEmpty();
            Context.UserNotifications.Should().HaveCountGreaterOrEqualTo(1);
            var notification = Context.UserNotifications.SingleOrDefault();
            notification.MessageText.Should().Contain("Your verification code");
            sendVerificationCodeResult.Should().BeOfType<OkResult>();
        }

        [Test]
        public void RequestPhoneNumberVerificationCodeAsync_EmptyPhoneNumberIsGiven_ShouldReturnBadRequest()
        {
            var user = CreateUserWithAvailableUserBonusPoints(0.0M);
            AuthorizeUser(user);

            var sendVerificationCodeResult = _accountController.RequestPhoneNumberVerificationCodeAsync(new PhoneNumberDto
            {
                PhoneNumber = string.Empty
            }).GetAwaiter().GetResult();

            Context.UserNotifications.Should().BeEmpty();
            Context.UserNotifications.Should().HaveCountGreaterOrEqualTo(0);
            Context.UserNotifications.SingleOrDefault().Should().BeNull();
            sendVerificationCodeResult.Should().BeOfType<BadRequestResult>();
        }

        [Test]
        public void ChangePhoneNumberAsync_PhoneNumberAndInvalidCodeIsGiven_ShouldReturnBadRequest()
        {
            var user = CreateUserWithAvailableUserBonusPoints(0.0M);
            AuthorizeUser(user);
            
            var changePhoneNumberResult = _accountController.ChangePhoneNumberAsync(new VerifyPhoneNumberDto
            {
                PhoneNumber = DefaultPhoneNumber,
                Code = InvalidCode
            }).GetAwaiter().GetResult();

            changePhoneNumberResult.Should().BeOfType<BadRequestObjectResult>();
            var changedPhoneNumberUser = Context.Users
                        .FirstOrDefault(u => u.PhoneNumber == DefaultPhoneNumber && u.UserName == DefaultPhoneNumber);
            changedPhoneNumberUser.Should().BeNull();
        }

        [Test]
        public void ChangePhoneNumberAsync_PhoneNumberAndCodeIsGiven_ShouldReturnOk()
        {
            var user = CreateUserWithAvailableUserBonusPoints(0.0M);
            AuthorizeUser(user);

            var code = GenerateVerificationCodeAsync(user, DefaultPhoneNumber).GetAwaiter().GetResult();

            var changePhoneNumberResult = _accountController.ChangePhoneNumberAsync(new VerifyPhoneNumberDto
            {
                PhoneNumber = DefaultPhoneNumber,
                Code = code
            }).GetAwaiter().GetResult();

            var changedPhoneNumberUser = Context.Users
                        .FirstOrDefault(u => u.PhoneNumber == DefaultPhoneNumber && u.UserName == DefaultPhoneNumber);
            changedPhoneNumberUser.Should().NotBeNull();
            changePhoneNumberResult.Should().BeOfType<OkResult>();
        }

        [Test]
        public void PatchUserAsync_UserInfoIsGiven_ShouldReturnOk()
        {
            var user = CreateUserWithAvailableUserBonusPoints(0.0M);
            AuthorizeUser(user);

            var userForm = new UserFormDto
            {
                Activity = (int)ActivityType.Medium,
                Pregnancy = (int)PregnancyType.No,
                Email = DefaultUserEmail,
                Goal = (int)GoalType.IncreaseWeight,
                Weight = 64,
                Height = 174,
                Age = 25,
                FullName = DefaultPhoneNumber
            };

            var patchUserResult = _accountController.PatchUserAsync(userForm).GetAwaiter().GetResult();

            var patchedUser = Context.Users
                .FirstOrDefault(u => u.FullName == DefaultPhoneNumber);

            patchedUser.Should().NotBeNull();
            patchUserResult.Should().BeOfType<OkResult>();
        }

        [Test]
        public void PatchUserAsync_UserInfoIsNotGiven_ShouldReturnOkButPatchedUserIsNotFound()
        {
            var user = CreateUserWithAvailableUserBonusPoints(0.0M);
            AuthorizeUser(user);

            var userForm = new UserFormDto();

            var patchUserResult = _accountController.PatchUserAsync(userForm).GetAwaiter().GetResult();

            var patchedUser = Context.Users
                .FirstOrDefault(u => u.FullName == DefaultPhoneNumber);

            patchedUser.Should().BeNull();
            patchUserResult.Should().BeOfType<OkResult>();
        }

        private static void EnsureUserFullInfoDtoIsCorrect(UserFullInfoDto userFullInfoDto, ApplicationUser referenceUser)
        {
            var dtoActivePaymentCard = userFullInfoDto.ActivePaymentCard;
            
            if (referenceUser.ActivePaymentCard != null && referenceUser.ActivePaymentCard.HasNumber)
            {
                dtoActivePaymentCard.Should().NotBeNull();
                dtoActivePaymentCard.Type.Should().NotBe(PaymentCardType.Unknown);
                dtoActivePaymentCard.Number.Should().NotBeNull();
                dtoActivePaymentCard.Number.LastFourDigits.Should().HaveLength(4);
                dtoActivePaymentCard.ExpirationDate.Should().NotBeNull();
            }
            else
            {
                dtoActivePaymentCard.Should().BeNull();
            }
            
            userFullInfoDto.PhoneNumberConfirmed.Should().Be(referenceUser.PhoneNumberConfirmed);
            userFullInfoDto.AvailableBonusPoints.Should().Be(referenceUser.TotalBonusPoints);
            userFullInfoDto.Email.Should().Be(referenceUser.Email);
            userFullInfoDto.UserName.Should().Be(referenceUser.UserName);
        }

        private void CreateUserActivePaymentCard(ref ApplicationUser user, bool includeNumberAndExpirationDate = true)
        {
            Context.Attach(user);
            
            Seeder.Seed(new PaymentCardsDataSet(user.Id));

            var paymentCard = Context.PaymentCards.OrderBy(pc => pc.Id).First();

            if (!includeNumberAndExpirationDate)
            {
                paymentCard.ResetNumberAndExpirationDate();
            }
            
            user.SetActivePaymentCard(paymentCard.Id);

            Context.SaveChanges();

            var userId = user.Id;
            user = Context.Users.Include(u => u.ActivePaymentCard).Single(u => u.Id == userId);
        }
        
        private ApplicationUser CreateUserWithAvailableUserBonusPoints(decimal availableUserBonuses)
        {
            Seeder.Seed(new UsersDataSet());

            var user = Context.Users.OrderBy(u => u.Id).First();
            user.PhoneNumberConfirmed = true;
            user.AddBonusPoints(availableUserBonuses, BonusType.Payment);

            Context.SaveChanges();

            return user;
        }

        private void AuthorizeUser(ApplicationUser user)
        {
            _accountController.ControllerContext = ControllerContextFactory.MakeForUserWithId(user.Id);
        }

        private async Task AddPaymentSystemForUser(ApplicationUser user)
        {
            var paymentCardNumber = new PaymentCardNumber("123456", "7890");
            var extendedPaymentCardInfo =
                new ExtendedPaymentCardInfo(paymentCardNumber, DateTime.UtcNow, DefaultCardToken);
            var paymentCard = new PaymentCard(user.Id, extendedPaymentCardInfo);
            Context.PaymentCards.Add(paymentCard);
            Context.SaveChanges();

            user.SetActivePaymentCard(extendedPaymentCardInfo);
            
            var result = await _userManager.UpdateAsync(user);
            result.Succeeded.Should().BeTrue();
        }

        private async Task<ApplicationUser> CreateUserAccordingToRegistrationStatusAsync(string userName, UserRegistrationStatus userRegistrationStatus)
        {
            var user = new ApplicationUser
            {
                UserName = userName
            };

            if (userRegistrationStatus == UserRegistrationStatus.PhoneNumberConfirmed ||
                userRegistrationStatus == UserRegistrationStatus.BankingCardConfirmed)
            {
                user.NotifyRegistrationCompletion();
            }

            if (userRegistrationStatus == UserRegistrationStatus.BankingCardConfirmed)
            {
                user.NotifyBankingCardVerificationInitiation();
                user.NotifyPaymentCardConfirmationCompletion();
            }

            var result = await _userManager.CreateAsync(user);

            if (userRegistrationStatus == UserRegistrationStatus.BankingCardConfirmed)
            {
                Seeder.Seed(new PaymentCardsDataSet(user.Id));
                user.SetProperty(
                    nameof(user.ActivePaymentCardId), Context.PaymentCards.AsNoTracking().First().Id
                );
                Context.Users.Attach(user);
                Context.SaveChanges();
            }
            
            result.Succeeded.Should().BeTrue();

            return user;
        }

        private async Task<string> GenerateVerificationCodeAsync(ApplicationUser user, string phoneNumber)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);

            var shortCode = user.ShortenChangePhoneTokenAndSaveRemainder(code);
            var result = await _userManager.UpdateAsync(user);

            result.Succeeded.Should().BeTrue();

            return shortCode;
        }
    }
}
