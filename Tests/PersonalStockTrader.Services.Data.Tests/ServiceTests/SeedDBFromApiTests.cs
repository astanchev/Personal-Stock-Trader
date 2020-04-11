namespace PersonalStockTrader.Services.Data.Tests.ServiceTests
{
    using System.Threading.Tasks;

    using Moq;
    using NUnit.Framework;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Services.CronJobs;

    [TestFixture]
    public class SeedDBFromApiTests
    {
        [Test]
        public async Task SeedDBFromApiWorksCorrectly()
        {
            var apiConnection = new Mock<IApiConnection>();
            apiConnection
                .Setup(x => x.GetCurrentData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var dbSeeder = new SeedDBFromApi(apiConnection.Object);

            await dbSeeder.Work();

            apiConnection.Verify(x => x.GetCurrentData(GlobalConstants.StockFunction, GlobalConstants.StockTicker, GlobalConstants.StockInterval), Times.Once);
        }
    }
}