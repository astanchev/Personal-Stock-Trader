namespace PersonalStockTrader.Services.Data.Tests.ServiceTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using MockQueryable.Moq;
    using Moq;
    using NUnit.Framework;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Services.Data.Tests.ServiceTests.Helpers;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;

    [TestFixture]
    public class AccountServiceTests
    {
        private Mock<IDeletableEntityRepository<Account>> accountRepository;
        private Mock<IQueryable<Account>> mock;
        private Mock<IPositionsService> positionService;
        private IAccountService accountService;

        [SetUp]
        public void Setup()
        {
            this.mock = TestDataHelpers.GetTestData().AsQueryable().BuildMock();

            this.accountRepository = new Mock<IDeletableEntityRepository<Account>>();

            this.accountRepository
                .Setup(a => a.All())
                .Returns(this.mock.Object);

            this.positionService = new Mock<IPositionsService>();

            this.positionService
                .Setup(p => p.OpenPosition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()));

            this.positionService
                .Setup(p => p.UpdatePosition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()));

            this.positionService
                .Setup(p => p.GetAccountClosedPositions(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestDataHelpers.GetTestPositionsTradeHistory);

            this.positionService
                .Setup(x => x.GetOpenPosition(1))
                .ReturnsAsync(TestDataHelpers.GetTestPosition());

            this.positionService
                .Setup(x => x.GetOpenPosition(2))
                .ReturnsAsync(() => null);

            this.accountService = new AccountService(this.accountRepository.Object, this.positionService.Object);
        }

        [Test]
        public async Task TakeAllAccountsMonthlyFeesAsyncShouldWorkCorrectly()
        {
            var expectedBalance = await this.accountRepository
                .Object
                .All()
                .Where(x => x.Id == 1)
                .Select(x => x.Balance)
                .FirstOrDefaultAsync();

            expectedBalance -= 50M;

            await this.accountService.TakeAllAccountsMonthlyFeesAsync();

            var actualBalance = await this.accountRepository
                .Object
                .All()
                .Where(x => x.Id == 1)
                .Select(x => x.Balance)
                .FirstOrDefaultAsync();

            Assert.AreEqual(expectedBalance, actualBalance);
        }

        [Test]
        public async Task ManagePositionsAsyncWithPositionId0InvokesCorrectMethod()
        {
            var testInput = new TradeSharesInputViewModel
            {
                AccountId = "1",
                PositionId = "0",
                Balance = "2000",
                CurrentPrice = "100.00",
                Quantity = "10",
                IsBuy = false,
            };

            await this.accountService.ManagePositionsAsync(testInput);

            this.positionService.Verify(x => x.OpenPosition(1, 10, false), Times.Once);
            this.positionService.Verify(x => x.UpdatePosition(1,  0, 10, false), Times.Never);
        }

        [Test]
        public async Task ManagePositionsAsyncWithPositionIdPositiveInvokesCorrectMethod()
        {
            var testInput = new TradeSharesInputViewModel
            {
                AccountId = "1",
                PositionId = "1",
                Balance = "2000",
                CurrentPrice = "100.00",
                Quantity = "10",
                IsBuy = false,
            };

            await this.accountService.ManagePositionsAsync(testInput);

            this.positionService.Verify(x => x.OpenPosition(1, 10, false), Times.Never);
            this.positionService.Verify(x => x.UpdatePosition(1,  1, 10, false), Times.Once);
        }

        [Test]
        public async Task ManagePositionsAsyncWithPositionIdNegativeReturnNull()
        {
            var testInput = new TradeSharesInputViewModel
            {
                AccountId = "1",
                PositionId = "-1",
                Balance = "2000",
                CurrentPrice = "100.00",
                Quantity = "10",
                IsBuy = false,
            };

            var result = await this.accountService.ManagePositionsAsync(testInput);

            Assert.Null(result);
        }

        [Test]
        public async Task GetCurrentPositionAsyncInvokesCorrectPositionServiceMethod()
        {
            await this.accountService.GetCurrentPositionAsync(1);

            this.positionService.Verify(x => x.GetOpenPosition(1), Times.Once);
        }

        [Test]
        public async Task GetCurrentPositionAsyncReturnCorrectlyIfPositionNull()
        {
            var returnedPosition = await this.accountService.GetCurrentPositionAsync(2);

            Assert.AreEqual(0, returnedPosition.PositionId);
        }

        [Test]
        public async Task GetCurrentPositionAsyncReturnCorrectlyIfPositionExists()
        {
            var returnedPosition = await this.accountService.GetCurrentPositionAsync(1);

            Assert.AreEqual(1, returnedPosition.PositionId);
        }

        [Test]
        [TestCase("1")]
        [TestCase("2")]
        public async Task GetAllClosedPositionsByUserIdAsyncReturnsCorrectData(string userId)
        {
           var result = await this.accountService.GetAllClosedPositionsByUserIdAsync(userId);

           Assert.AreEqual(2, result.Positions.Count());
        }

        [Test]
        public async Task GetAllClosedPositionsByUserIdAsyncInvokesPositionServiceMethod()
        {
            var result = await this.accountService.GetAllClosedPositionsByUserIdAsync("1");

            this.positionService.Verify(x => x.GetAccountClosedPositions(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        [TestCase("1")]
        [TestCase("2")]
        public async Task GetAllClosedPositionsIntervalByUserIdAsyncReturnsCorrectData(string userId)
        {
            var result = await this.accountService.GetAllClosedPositionsIntervalByUserIdAsync(userId, DateTime.Parse("01.01.2020").ToShortDateString(), DateTime.Now.ToShortDateString());

            Assert.AreEqual(2, result.Positions.Count());
        }

        [Test]
        public async Task GetAllClosedPositionsIntervalByUserIdAsyncInvokesPositionServiceMethod()
        {
            var result = await this.accountService.GetAllClosedPositionsIntervalByUserIdAsync("1", DateTime.Parse("01.01.2020").ToShortDateString(), DateTime.Now.ToShortDateString());

            this.positionService.Verify(x => x.GetAccountClosedPositions(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
