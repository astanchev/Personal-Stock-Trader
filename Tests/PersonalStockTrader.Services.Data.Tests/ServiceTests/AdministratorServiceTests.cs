namespace PersonalStockTrader.Services.Data.Tests.ServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.AspNetCore.Identity;
    using Moq;
    using NUnit.Framework;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Data.Repositories;

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
    }
}
