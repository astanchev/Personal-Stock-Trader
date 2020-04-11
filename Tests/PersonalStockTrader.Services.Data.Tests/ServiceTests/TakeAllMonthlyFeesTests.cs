namespace PersonalStockTrader.Services.Data.Tests.ServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MockQueryable.Moq;
    using Moq;
    using NUnit.Framework;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Services.CronJobs;

    [TestFixture]
    public class TakeAllMonthlyFeesTests
    {
        [Test]
        public async Task TakeAllMonthlyFeesWorksCorrectly()
        {
            var mock = new List<Account>().AsQueryable().BuildMock();

            var accountRepository = new Mock<IDeletableEntityRepository<Account>>();
            var positionService = new Mock<IPositionsService>();
            var accountService = new AccountService(accountRepository.Object, positionService.Object);

            accountRepository
                .Setup(x => x.All())
                .Returns(mock.Object);

            var takeAllMonthlyFeesService = new TakeAllMonthlyFees(accountService);
            await takeAllMonthlyFeesService.Work();

            accountRepository.Verify(x => x.All(), Times.Once);
        }
    }
}