namespace PersonalStockTrader.Services.Data.Tests.ServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Moq;
    using NUnit.Framework;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Data.Repositories;
    using PersonalStockTrader.Services.Data.Tests.ServiceTests.Helpers;
    using Web.ViewModels.Administration.Dashboard;

    [TestFixture]
    public class AdministratorServiceTests
    {
        private EfDeletableEntityRepository<ApplicationUser> userRepository;
        private Mock<UserManager<ApplicationUser>> userManager;
        private IAdministratorService administratorService;

        [SetUp]
        public async Task Setup()
        {
            var context = ApplicationDbContextInMemoryFactory.InitializeContext();
            this.userRepository = new EfDeletableEntityRepository<ApplicationUser>(context);

            var mockUsers = TestDataHelpers.GetTestUsers();
            foreach (var user in mockUsers)
            {
                await this.userRepository.AddAsync(user);
                await this.userRepository.SaveChangesAsync();
            }

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            this.userManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
            this.userManager
                .Setup(m => m.GetUsersInRoleAsync(GlobalConstants.AccountManagerRoleName))
                .ReturnsAsync(mockUsers);
            this.userManager
                .Setup(u => u.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), GlobalConstants.AccountManagerRoleName))
                .ReturnsAsync(IdentityResult.Success);
            this.userManager
                .Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), GlobalConstants.AccountManagerRoleName))
                .ReturnsAsync(IdentityResult.Success);
            this.userManager
                .Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            this.userManager
                .Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            this.administratorService = new AdministratorService(this.userManager.Object, this.userRepository);
        }

        [Test]
        public async Task GetAllAccountManagersAsyncCallsUserManagerForAccountManager()
        {
            var result = await this.administratorService.GetAllAccountManagersAsync();

            this.userManager.Verify(m => m.GetUsersInRoleAsync(GlobalConstants.AccountManagerRoleName), Times.Once);
        }

        [Test]
        public async Task GetAllAccountManagersAsyncReturnsCorrectData()
        {
            var result = await this.administratorService.GetAllAccountManagersAsync();

            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetAccountManagersByIdAsyncReturnsCorrectDataWithCorrectId()
        {
            var result = await this.administratorService.GetAccountManagersByIdAsync("1");

            Assert.NotNull(result);
            Assert.AreEqual("1", result.UserId);
        }

        [Test]
        public void GetAccountManagersByIdAsyncThrowsWithNotCorrectId()
        {
            Assert.ThrowsAsync<NullReferenceException>(async () => await this.administratorService.GetAccountManagersByIdAsync("A"));
        }

        [Test]
        public void RemoveAccountManagerAsyncShouldThrowIfUserNotFound()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await this.administratorService.RemoveAccountManagerAsync("A"));
        }

        [Test]
        public void RemoveAccountManagerAsyncShouldReturnSuccess()
        {
            var result = this.administratorService.RemoveAccountManagerAsync("1");

            Assert.IsTrue(result.IsCompletedSuccessfully);
        }

        [Test]
        public async Task RemoveAccountManagerAsyncShouldCallUserManagerRemove()
        {
            var result = await this.administratorService.RemoveAccountManagerAsync("1");

            this.userManager.Verify(u => u.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), GlobalConstants.AccountManagerRoleName), Times.Once);
        }

        [Test]
        public void CreateAccountManagerAsyncShouldReturnSuccess()
        {
            var manager = new AccountManagerInputViewModel
            {
                Username = "C",
                Email = "c@c.c",
                Password = "123456",
            };

            var result = this.administratorService.CreateAccountManagerAsync(manager);

            Assert.IsTrue(result.IsCompletedSuccessfully);
        }

        [Test]
        public async Task CreateAccountManagerAsyncShouldCallUserManagerAdd()
        {
            var manager = new AccountManagerInputViewModel
            {
                Username = "C",
                Email = "c@c.c",
                Password = "123456",
            };

            var result = await this.administratorService.CreateAccountManagerAsync(manager);

            this.userManager.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), GlobalConstants.AccountManagerRoleName), Times.Once);
        }

        [Test]
        public async Task CreateAccountManagerAsyncShouldCallUserManagerCreate()
        {
            var manager = new AccountManagerInputViewModel
            {
                Username = "C",
                Email = "c@c.c",
                Password = "123456",
            };

            var result = await this.administratorService.CreateAccountManagerAsync(manager);

            this.userManager.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task UpdateAccountManagerAsyncWorkCorrectlyWithCorrectData()
        {
            var managerToUpdate = new AccountManagerOutputViewModel
            {
                UserId = "1",
                Username = "D",
                Email = "d@d.d",
            };

            var result = await this.administratorService.UpdateAccountManagerAsync(managerToUpdate);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task UpdateAccountManagerAsyncUpdatesCorrectlyWithCorrectData()
        {
            var managerToUpdate = new AccountManagerOutputViewModel
            {
                UserId = "1",
                Username = "D",
                Email = "d@d.d",
            };

            var result = await this.administratorService.UpdateAccountManagerAsync(managerToUpdate);

            var updatedManager = this.userRepository
                .All()
                .FirstOrDefault(u => u.Id == "1");

            Assert.AreEqual(managerToUpdate.Username, updatedManager?.UserName);
            Assert.AreEqual(managerToUpdate.Email, updatedManager?.Email);
        }

        [Test]
        public async Task UpdateAccountManagerAsyncReturnsFalseWithInCorrectData()
        {
            var managerToUpdate = new AccountManagerOutputViewModel
            {
                UserId = "-1",
                Username = "D",
                Email = "d@d.d",
            };

            var result = await this.administratorService.UpdateAccountManagerAsync(managerToUpdate);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task UpdateAccountManagerAsyncShouldCallUserManagerUpdate()
        {
            var managerToUpdate = new AccountManagerOutputViewModel
            {
                UserId = "1",
                Username = "D",
                Email = "d@d.d",
            };

            var result = await this.administratorService.UpdateAccountManagerAsync(managerToUpdate);

            this.userManager.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }
    }
}