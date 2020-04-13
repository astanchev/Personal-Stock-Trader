namespace PersonalStockTrader.Services.Data.Tests.ServiceTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MockQueryable.Moq;
    using Moq;
    using NUnit.Framework;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Services.Data.Tests.ServiceTests.Helpers;
    using PersonalStockTrader.Web.ViewModels.User.TradeHistory;

    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IDeletableEntityRepository<Account>> accountRepository;
        private Mock<IQueryable<Account>> mock;
        private Mock<IQueryable<HistoryPositionViewModel>> mockPositions;
        private IUserService userService;
        private Mock<IPositionsService> positionService;

        [SetUp]
        public void Setup()
        {
            this.mock = TestDataHelpers.GetTestData().AsQueryable().BuildMock();
            this.mockPositions = TestDataHelpers.GetTestHistoryPositions().AsQueryable().BuildMock();

            this.accountRepository = new Mock<IDeletableEntityRepository<Account>>();
            this.accountRepository
                .Setup(a => a.All())
                .Returns(this.mock.Object);

            this.positionService = new Mock<IPositionsService>();
            this.positionService
                .Setup(p => p.GetAccountClosedPositions(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(this.mockPositions.Object);

            this.userService = new UserService(this.accountRepository.Object, this.positionService.Object);
        }

        [Test]
        public async Task GetUserBalanceAsyncWithCorrectDataShouldReturnCorrectly()
        {
            var actualUserBalance = await this.userService.GetUserBalanceAsync("1");

            Assert.AreEqual(2000M, actualUserBalance);
        }

        [Test]
        public async Task GetUserBalanceAsyncForNonExistingUserShouldReturnZero()
        {
            var actualUserBalance = await this.userService.GetUserBalanceAsync("3");

            Assert.AreEqual(0M, actualUserBalance);
        }

        [Test]
        public async Task GetUserAccountIdAsyncWithCorrectDataShouldReturnCorrectly()
        {
            var actualUserBalance = await this.userService.GetUserAccountIdAsync("1");

            Assert.AreEqual(1, actualUserBalance);
        }

        [Test]
        public async Task GetUserAccountIdAsyncForNonExistingUserShouldReturnZero()
        {
            var actualUserBalance = await this.userService.GetUserAccountIdAsync("3");

            Assert.AreEqual(0, actualUserBalance);
        }

        [Test]
        public async Task GetUserPaidTradeFeesShouldReturnNotEmptyCollectionWithCorrectData()
        {
            var result = await this.userService.GetUserPaidTradeFees("1", "01.04.2020", "10.04.2020");

            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(10, result.Count);
        }

        [Test]
        public async Task GetUserPaidTradeFeesShouldReturnCorrectData()
        {
            var result = await this.userService.GetUserPaidTradeFees("1", "01.04.2020", "10.04.2020");

            var expectedSum = 100.00M;
            var actualSum = result.Values.Sum();

            Assert.AreEqual(expectedSum, actualSum);
        }

        [Test]
        public async Task GetUserPaidMonthlyCommissionsShouldReturnNotEmptyCollectionWithCorrectData()
        {
            var result = await this.userService.GetUserPaidMonthlyFees("1", "01.04.2020", "10.04.2020");

            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(10, result.Count);
        }

        [Test]
        public async Task GetUserPaidMonthlyCommissionsShouldReturnCorrectData()
        {
            var result = await this.userService.GetUserPaidMonthlyFees("1", "01.04.2020", "10.04.2020");

            var expectedSum = 100.00M;
            var actualSum = result.Values.Sum();

            Assert.AreEqual(expectedSum, actualSum);
        }

        [Test]
        public async Task GetUserProfitLossShouldReturnNotEmptyCollectionWithCorrectData()
        {
            var result = await this.userService.GetUserProfitLoss("1", "01.04.2020", "10.04.2020");

            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(10, result.Count);
        }

        [Test]
        public async Task GetUserProfitLossShouldReturnCorrectData()
        {
            var result = await this.userService.GetUserProfitLoss("1", "01.04.2020", "10.04.2020");

            var expectedSum = 30.00M;
            var actualSum = result.Values.Sum();

            Assert.AreEqual(expectedSum, actualSum);
        }
    }
}
