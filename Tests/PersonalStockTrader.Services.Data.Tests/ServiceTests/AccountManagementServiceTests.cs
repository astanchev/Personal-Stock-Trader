namespace PersonalStockTrader.Services.Data.Tests.ServiceTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.AspNetCore.Identity;
    using MockQueryable.Moq;
    using Moq;
    using NUnit.Framework;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Data.Repositories;
    using PersonalStockTrader.Services.Data.Tests.ServiceTests.Helpers;

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

        [SetUp]
        public void Setup()
        {
            this.mockNotConfirmedUsers = TestDataHelpers.GetTestNotConfirmedUsers();
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            this.userManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
            this.userManager
                .Setup(u => u.GetUsersInRoleAsync(GlobalConstants.NotConfirmedUserRoleName))
                .ReturnsAsync(this.mockNotConfirmedUsers);

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

            this.positionService = new Mock<IPositionsService>();
            this.feePaymentsRepository = new Mock<IRepository<FeePayment>>();

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

            this.userRepository.Verify(a => a.AllWithDeleted(), Times.Once);
        }

        [Test]
        public async Task GetClientToBeConfirmedByIdAsyncWithWrongIdShouldReturnNull()
        {
            var result = await this.accountManagementService.GetClientToBeConfirmedByIdAsync("-1");

            Assert.IsNull(result);
        }
    }
}
