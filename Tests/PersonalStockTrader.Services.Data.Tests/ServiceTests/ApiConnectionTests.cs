namespace PersonalStockTrader.Services.Data.Tests.ServiceTests
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Moq;
    using NUnit.Framework;
    using PersonalStockTrader.Common;

    [TestFixture]
    public class ApiConnectionTests
    {
        [Test]
        public async Task GetCurrentDataInvokesStockService()
        {
            var stockService = new Mock<IStockService>();
            stockService
                .Setup(x => x.GetLastUpdatedTime(It.IsAny<string>()))
                .ReturnsAsync(DateTime.Now);

            stockService
                .Setup(x => x.ImportData(It.IsAny<string>(), GlobalConstants.StockTicker));

            var mockConfiguration = new Mock<IConfiguration>();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(s => s.GetService(typeof(IConfiguration)))
                .Returns(mockConfiguration.Object);

            var apiConnection = new AlphaVantageApiClient(stockService.Object, serviceProvider.Object);

            await apiConnection.GetCurrentData(GlobalConstants.StockFunction, GlobalConstants.StockTicker, GlobalConstants.StockInterval);

            stockService.Verify(s => s.GetLastUpdatedTime(GlobalConstants.StockTicker), Times.Once);
            stockService.Verify(s => s.ImportData(It.IsAny<string>(), GlobalConstants.StockTicker));
        }
    }
}