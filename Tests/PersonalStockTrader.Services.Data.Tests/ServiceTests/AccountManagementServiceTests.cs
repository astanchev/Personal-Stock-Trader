namespace PersonalStockTrader.Services.Data.Tests.ServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.Configuration.Annotations;
    using Microsoft.AspNetCore.Identity;
    using MockQueryable.Moq;
    using Moq;
    using NUnit.Framework;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Data.Repositories;
    using PersonalStockTrader.Services.Data.Tests.ServiceTests.Helpers;
    using PersonalStockTrader.Web.ViewModels.AccountManagement.NewClients;

    [TestFixture]
    public class AccountManagementServiceTests
    {
        private Mock<IDeletableEntityRepository<ApplicationUser>> userRepository;
        private Mock<UserManager<ApplicationUser>> userManager;
        private Mock<IDeletableEntityRepository<Account>> accountRepository;
        private Mock<IPositionsService> positionService;
        private Mock<IRepository<FeePayment>> feePaymentsRepository;
        private IAccountManagementService accountManagementService;
        private Mock<IQueryable<Account>> mock;
        private Mock<IQueryable<ApplicationUser>> mockUsers;
        private List<ApplicationUser> mockNotConfirmedUsers;
        private Mock<IUserStore<ApplicationUser>> userStoreMock;
        private Mock<IQueryable<FeePayment>> mockFees;

        [SetUp]
        public void Setup()
        {
            this.mockNotConfirmedUsers = TestDataHelpers.GetTestNotConfirmedUsers();
            this.userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            this.userManager = new Mock<UserManager<ApplicationUser>>(this.userStoreMock.Object, null, null, null, null, null, null, null, null);
            this.userManager
                .Setup(u => u.GetUsersInRoleAsync(GlobalConstants.NotConfirmedUserRoleName))
                .ReturnsAsync(this.mockNotConfirmedUsers);
            this.userManager
                .Setup(
                    u => u.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            this.userManager
                .Setup(
                    u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            this.mock = TestDataHelpers.GetTestData().AsQueryable().BuildMock();
            this.accountRepository = new Mock<IDeletableEntityRepository<Account>>();
            this.accountRepository
                .Setup(a => a.All())
                .Returns(this.mock.Object);
            this.accountRepository
                .Setup(a => a.AllWithDeleted())
                .Returns(this.mock.Object);

            this.mockUsers = TestDataHelpers.GetTestUsers().AsQueryable().BuildMock();
            this.userRepository = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            this.userRepository
                .Setup(a => a.AllWithDeleted())
                .Returns(this.mockUsers.Object);

            this.mockFees = TestDataHelpers.GetTestFeePayments().AsQueryable().BuildMock();
            this.feePaymentsRepository = new Mock<IRepository<FeePayment>>();
            this.feePaymentsRepository
                .Setup(f => f.All())
                .Returns(this.mockFees.Object);

            this.positionService = new Mock<IPositionsService>();
            this.positionService
                .Setup(p => p.ClosePosition(It.IsAny<int>()));

            this.accountManagementService = new AccountManagementService(this.userManager.Object, this.userRepository.Object, this.accountRepository.Object, this.positionService.Object, this.feePaymentsRepository.Object);
        }

        [Test]
        public async Task GetAllConfirmedAccountsShouldReturnCorrectData()
        {
            var result = await this.accountManagementService.GetAllNotConfirmedClientsAsync();

            Assert.IsNotNull(result);
            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllNotConfirmedAccountsShouldCallAccountRepository()
        {
            var result = await this.accountManagementService.GetAllNotConfirmedClientsAsync();

            this.userManager.Verify(u => u.GetUsersInRoleAsync(GlobalConstants.NotConfirmedUserRoleName), Times.Once);
        }

        [Test]
        public void GetAllNotConfirmedAccountsShouldReturnCorrectData()
        {
            var result = this.accountManagementService.GetAllConfirmedAccounts();

            Assert.IsNotNull(result);
            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetAllConfirmedAccountsShouldCallUserManager()
        {
            var result = this.accountManagementService.GetAllConfirmedAccounts();

            this.accountRepository.Verify(a => a.AllWithDeleted(), Times.Once);
        }

        [Test]
        public async Task GetClientToBeConfirmedByIdAsyncShouldReturnCorrectData()
        {
            var result = await this.accountManagementService.GetClientToBeConfirmedByIdAsync("1");

            Assert.IsNotNull(result);
            Assert.AreEqual("1", result.UserId);
        }

        [Test]
        public async Task GetAllConfirmedAccountsShouldCallUserRepository()
        {
            var result = await this.accountManagementService.GetClientToBeConfirmedByIdAsync("1");

            this.userRepository.Verify(u => u.AllWithDeleted(), Times.Once);
        }

        [Test]
        public async Task GetClientToBeConfirmedByIdAsyncWithWrongIdShouldReturnNull()
        {
            var result = await this.accountManagementService.GetClientToBeConfirmedByIdAsync("-1");

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetClientToBeManagedByAccountIdAsyncShouldReturnCorrectData()
        {
            var result = await this.accountManagementService.GetClientToBeManagedByAccountIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.AccountId);
        }

        [Test]
        public async Task GetClientToBeManagedByAccountIdAsyncShouldCallAccountRepository()
        {
            var result = await this.accountManagementService.GetClientToBeManagedByAccountIdAsync(1);

            this.accountRepository.Verify(a => a.AllWithDeleted(), Times.Once);
        }

        [Test]
        public async Task GetClientToBeManagedByAccountIdAsyncWithWrongIdShouldReturnNull()
        {
            var result = await this.accountManagementService.GetClientToBeManagedByAccountIdAsync(-1);

            Assert.IsNull(result);
        }

        [Test]
        public void GetPaidTradeFeesLast7DaysShouldReturnCorrectData()
        {
            var result = this.accountManagementService.GetPaidTradeFeesLast7Days();

            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(7, result.Count);
        }

        [Test]
        public void GetPaidTradeFeesLast7DaysShouldCallFeePaymentsRepository()
        {
            var result = this.accountManagementService.GetPaidTradeFeesLast7Days();

            this.feePaymentsRepository.Verify(a => a.All(), Times.Once);
        }

        [Test]
        public void GetPaidMonthlyFeesLast6MonthsShouldReturnCorrectData()
        {
            var result = this.accountManagementService.GetPaidMonthlyFeesLast6Months();

            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(6, result.Count);
        }

        [Test]
        public void GetPaidMonthlyFeesLast6MonthsShouldCallFeePaymentsRepository()
        {
            var result = this.accountManagementService.GetPaidMonthlyFeesLast6Months();

            this.feePaymentsRepository.Verify(a => a.All(), Times.Once);
        }

        [Test]
        public void GetAllPaidFeesLast90DaysShouldReturnCorrectData()
        {
            var result = this.accountManagementService.GetAllPaidFeesLast90Days();

            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(90, result.Count);
        }

        [Test]
        public void GetAllPaidFeesLast90DaysShouldCallFeePaymentsRepository()
        {
            var result = this.accountManagementService.GetAllPaidFeesLast90Days();

            this.feePaymentsRepository.Verify(a => a.All(), Times.Once);
        }

        [Test]
        public void GetAllNewUsersLast90DaysShouldReturnCorrectData()
        {
            var testAccountManagementService = this.GetAcctManServiceWithTestUserManager(out var testUserManager);

            var result = testAccountManagementService.GetAllNewUsersLast90Days();

            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(90, result.Count);
        }

        [Test]
        public void GetAllNewUsersLast90DaysShouldCallUserManager()
        {
            var testAccountManagementService = this.GetAcctManServiceWithTestUserManager(out var testUserManager);

            var result = testAccountManagementService.GetAllNewUsersLast90Days();

            testUserManager.Verify(a => a.Users, Times.Once);
        }

        [Test]
        public async Task ConfirmUserAccountAsyncWorkCorrectly()
        {
            var context = ApplicationDbContextInMemoryFactory.InitializeContext();
            var testUserRepository = new EfDeletableEntityRepository<ApplicationUser>(context);
            var testNotConfirmedUsers = TestDataHelpers.GetTestNotConfirmedUsers();
            var testAccountManagementService = new AccountManagementService(this.userManager.Object, testUserRepository, this.accountRepository.Object, this.positionService.Object, this.feePaymentsRepository.Object);

            foreach (var tester in testNotConfirmedUsers)
            {
                await testUserRepository.AddAsync(tester);
                await testUserRepository.SaveChangesAsync();
            }

            var user = new ClientToBeConfirmedViewModel
            {
                UserId = "11",
                Username = "A",
                Email = "a@a.a",
                Balance = 5000,
                TradeFee = 50,
                MonthlyFee = 100,
                Notes = null,
            };

            var testUser = testUserRepository.All().FirstOrDefault(u => u.Id == "11");

            Assert.IsNull(testUser.Account);

            await testAccountManagementService.ConfirmUserAccountAsync(user);

            Assert.IsNotNull(testUser.Account);
        }

        [Test]
        public async Task ConfirmUserAccountAsyncShouldCallUserManager()
        {
            var context = ApplicationDbContextInMemoryFactory.InitializeContext();
            var testUserRepository = new EfDeletableEntityRepository<ApplicationUser>(context);
            var testNotConfirmedUsers = TestDataHelpers.GetTestNotConfirmedUsers();
            var testAccountManagementService = new AccountManagementService(this.userManager.Object, testUserRepository, this.accountRepository.Object, this.positionService.Object, this.feePaymentsRepository.Object);

            foreach (var tester in testNotConfirmedUsers)
            {
                await testUserRepository.AddAsync(tester);
                await testUserRepository.SaveChangesAsync();
            }

            var user = new ClientToBeConfirmedViewModel
            {
                UserId = "11",
                Username = "A",
                Email = "a@a.a",
                Balance = 5000,
                TradeFee = 50,
                MonthlyFee = 100,
                Notes = null,
            };

            await testAccountManagementService.ConfirmUserAccountAsync(user);

            this.userManager.Verify(u => u.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), GlobalConstants.NotConfirmedUserRoleName), Times.Once);
            this.userManager.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), GlobalConstants.ConfirmedUserRoleName), Times.Once);
        }

        [Test]
        public async Task DeleteUserAccountAsyncShouldWorkCorrectly()
        {
            var testAccountRepository = await GetTestAcctRepository();

            var testAccountManagementService = new AccountManagementService(this.userManager.Object, this.userRepository.Object, testAccountRepository, this.positionService.Object, this.feePaymentsRepository.Object);

            await testAccountManagementService.DeleteUserAccountAsync("1", 1);

            var account = testAccountRepository
                .AllWithDeleted()
                .FirstOrDefault(a => a.Id == 1);

            Assert.IsTrue(account.IsDeleted);
        }

        [Test]
        public async Task DeleteUserAccountAsyncShouldCallUserManager()
        {
            var testAccountRepository = await GetTestAcctRepository();

            var testAccountManagementService = new AccountManagementService(this.userManager.Object, this.userRepository.Object, testAccountRepository, this.positionService.Object, this.feePaymentsRepository.Object);

            await testAccountManagementService.DeleteUserAccountAsync("1", 1);

            this.userManager.Verify(u => u.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            this.userManager.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task RestoreUserAccountAsyncShouldWorkCorrectly()
        {
            var context = ApplicationDbContextInMemoryFactory.InitializeContext();
            var testAccountRepository = new EfDeletableEntityRepository<Account>(context);

            var testAccounts = TestDataHelpers.GetTestData();
            foreach (var testAccount in testAccounts)
            {
                if (testAccount.Id == 1)
                {
                    testAccount.IsDeleted = true;
                }

                await testAccountRepository.AddAsync(testAccount);
                await testAccountRepository.SaveChangesAsync();
            }

            var testAccountManagementService = new AccountManagementService(this.userManager.Object, this.userRepository.Object, testAccountRepository, this.positionService.Object, this.feePaymentsRepository.Object);

            await testAccountManagementService.RestoreUserAccountAsync("1", 1);

            var account = testAccountRepository
                .AllWithDeleted()
                .FirstOrDefault(a => a.Id == 1);

            Assert.IsFalse(account.IsDeleted);
        }

        [Test]
        public async Task RestoreUserAccountAsyncShouldCallUserManager()
        {
            var context = ApplicationDbContextInMemoryFactory.InitializeContext();
            var testAccountRepository = new EfDeletableEntityRepository<Account>(context);

            var testAccounts = TestDataHelpers.GetTestData();
            foreach (var testAccount in testAccounts)
            {
                if (testAccount.Id == 1)
                {
                    testAccount.IsDeleted = true;
                }

                await testAccountRepository.AddAsync(testAccount);
                await testAccountRepository.SaveChangesAsync();
            }

            var testAccountManagementService = new AccountManagementService(this.userManager.Object, this.userRepository.Object, testAccountRepository, this.positionService.Object, this.feePaymentsRepository.Object);

            await testAccountManagementService.RestoreUserAccountAsync("1", 1);

            this.userManager.Verify(u => u.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            this.userManager.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task UpdateUserAccountAsyncShouldWorkCorrectly()
        {
            var testAccountRepository = await GetTestAcctRepository();

            var testAccountManagementService = new AccountManagementService(this.userManager.Object, this.userRepository.Object, testAccountRepository, this.positionService.Object, this.feePaymentsRepository.Object);

            await testAccountManagementService.UpdateUserAccountAsync("1", 10000M, 80M, 80M);

            var account = testAccountRepository
                .AllWithDeleted()
                .FirstOrDefault(a => a.Id == 1);

            Assert.AreEqual(80M, account.TradeFee);
            Assert.AreEqual(80M, account.MonthlyFee);
            Assert.AreEqual(10000M, account.Balance);
        }

        [Test]
        public void UpdateUserAccountAsyncShouldNotWorkWithWrongData()
        {
            var testAccountRepository = GetTestAcctRepository().GetAwaiter().GetResult();

            var testAccountManagementService = new AccountManagementService(this.userManager.Object, this.userRepository.Object, testAccountRepository, this.positionService.Object, this.feePaymentsRepository.Object);

            Assert.ThrowsAsync<NullReferenceException>(async () =>
                await testAccountManagementService.UpdateUserAccountAsync("-1", 10000M, 80M, 80M));
        }

        [Test]
        public async Task DeleteUserAsyncShouldDeleteUserCorrectly()
        {
            var context = ApplicationDbContextInMemoryFactory.InitializeContext();

            var testUserRepository = new EfDeletableEntityRepository<ApplicationUser>(context);
            var testUsers = TestDataHelpers.GetTestUsers();
            foreach (var tester in testUsers)
            {
                await testUserRepository.AddAsync(tester);
                await testUserRepository.SaveChangesAsync();
            }

            var testAccountRepository = new Mock<IDeletableEntityRepository<Account>>();
            testAccountRepository
                .Setup(a => a.GetByIdWithDeletedAsync(It.IsAny<int>()))
                .ReturnsAsync(new Account()
                {
                    Id = 1,
                    IsDeleted = false,
                });
            testAccountRepository
                .Setup(a => a.Delete(It.IsAny<Account>()));
            testAccountRepository
                .Setup(a => a.SaveChangesAsync());

            var testAccountManagementService = new AccountManagementService(this.userManager.Object, testUserRepository, testAccountRepository.Object, this.positionService.Object, this.feePaymentsRepository.Object);

            await testAccountManagementService.DeleteUserAsync("1", 1);
            var user = testUserRepository.AllWithDeleted().FirstOrDefault(u => u.Id == "1");

            Assert.IsTrue(user.IsDeleted);
        }

        [Test]
        public async Task DeleteUserAsyncShouldDeleteUserAccountCorrectly()
        {
            var context = ApplicationDbContextInMemoryFactory.InitializeContext();

            var testAccountRepository = new EfDeletableEntityRepository<Account>(context);
            var testAccounts = TestDataHelpers.GetTestData();
            foreach (var acc in testAccounts)
            {
                await testAccountRepository.AddAsync(acc);
                await testAccountRepository.SaveChangesAsync();
            }

            var testUserRepository = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            testUserRepository
                .Setup(a => a.GetByIdWithDeletedAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser()
                {
                    Id = "1",
                    IsDeleted = false,
                });
            testUserRepository
                .Setup(a => a.Delete(It.IsAny<ApplicationUser>()));
            testUserRepository
                .Setup(a => a.SaveChangesAsync());

            var testAccountManagementService = new AccountManagementService(this.userManager.Object, testUserRepository.Object, testAccountRepository, this.positionService.Object, this.feePaymentsRepository.Object);

            await testAccountManagementService.DeleteUserAsync("1", 1);
            var account = testAccountRepository.AllWithDeleted().FirstOrDefault(a => a.Id == 1);

            Assert.IsTrue(account.IsDeleted);
        }

        [Test]
        public async Task DeleteUserAsyncShouldThrowException()
        {
            var context = ApplicationDbContextInMemoryFactory.InitializeContext();

            var testUserRepository = new EfDeletableEntityRepository<ApplicationUser>(context);
            var testUsers = TestDataHelpers.GetTestUsers();
            foreach (var tester in testUsers)
            {
                await testUserRepository.AddAsync(tester);
                await testUserRepository.SaveChangesAsync();
            }

            var testAccountRepository = new Mock<IDeletableEntityRepository<Account>>();

            var testAccountManagementService = new AccountManagementService(this.userManager.Object, testUserRepository, testAccountRepository.Object, this.positionService.Object, this.feePaymentsRepository.Object);

            Assert.ThrowsAsync<NullReferenceException>(async () => await testAccountManagementService.DeleteUserAsync("-1", 1));
        }

        private static async Task<EfDeletableEntityRepository<Account>> GetTestAcctRepository()
        {
            var context = ApplicationDbContextInMemoryFactory.InitializeContext();
            var testAccountRepository = new EfDeletableEntityRepository<Account>(context);

            var testAccounts = TestDataHelpers.GetTestData();
            foreach (var testAccount in testAccounts)
            {
                await testAccountRepository.AddAsync(testAccount);
                await testAccountRepository.SaveChangesAsync();
            }

            return testAccountRepository;
        }

        private AccountManagementService GetAcctManServiceWithTestUserManager(out Mock<UserManager<ApplicationUser>> testUserManager)
        {
            var testUserStoreMock = new Mock<IQueryableUserStore<ApplicationUser>>();
            testUserManager = new Mock<UserManager<ApplicationUser>>(
                testUserStoreMock.Object, null, null, null, null, null, null, null, null);
            testUserManager
                .Setup(u => u.Users)
                .Returns(this.mockUsers.Object);
            var testAccountManagementService = new AccountManagementService(testUserManager.Object, this.userRepository.Object, this.accountRepository.Object, this.positionService.Object, this.feePaymentsRepository.Object);
            return testAccountManagementService;
        }
    }
}
