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

    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IDeletableEntityRepository<Account>> accountRepository;
        private Mock<IQueryable<Account>> mock;
        private IUserService userService;

        [SetUp]
        public void Setup()
        {
            this.mock = this.GetTestData().AsQueryable().BuildMock();

            this.accountRepository = new Mock<IDeletableEntityRepository<Account>>();
            this.accountRepository
                .Setup(a => a.All())
                .Returns(this.mock.Object);

            this.userService = new UserService(this.accountRepository.Object);
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

        private List<Account> GetTestData()
        {
            return new List<Account>()
            {
                new Account
                {
                    Id = 1,
                    UserId = "1",
                    Balance = 2000M,
                },
                new Account
                {
                    Id = 2,
                    UserId = "2",
                    Balance = 5000M,
                },
            };
        }
    }
}