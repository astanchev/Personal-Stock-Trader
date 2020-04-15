namespace PersonalStockTrader.Services.Data.Tests.ServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using Helpers;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using MockQueryable.Moq;
    using Models;
    using Moq;
    using NUnit.Framework;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;

    [TestFixture]
    public class StockServiceTests
    {
        private Mock<IDeletableEntityRepository<Stock>> stockRepository;
        private Mock<IDeletableEntityRepository<DataSet>> datasetRepository;
        private Mock<IDeletableEntityRepository<MetaData>> metadataRepository;
        private Mock<IDeletableEntityRepository<Interval>> intervalRepository;
        private IMemoryCache memoryCache;
        private IStockService stockService;

        [SetUp]
        public void Setup()
        {
            this.stockRepository = new Mock<IDeletableEntityRepository<Stock>>();
            this.datasetRepository = new Mock<IDeletableEntityRepository<DataSet>>();
            this.metadataRepository = new Mock<IDeletableEntityRepository<MetaData>>();
            this.intervalRepository = new Mock<IDeletableEntityRepository<Interval>>();
            this.memoryCache = new MemoryCache(new MemoryCacheOptions());

            var mockStocks = TestDataHelpers.GetTestStocks().AsQueryable().BuildMock();
            this.stockRepository
                .Setup(s => s.All())
                .Returns(mockStocks.Object);
            this.stockRepository
                .Setup(d => d.SaveChangesAsync());

            var mockIntervals = TestDataHelpers.GetTestIntervals().AsQueryable().BuildMock();
            this.intervalRepository
                .Setup(i => i.All())
                .Returns(mockIntervals.Object);

            var mockDatasets = TestDataHelpers.GetTestDataSets().AsQueryable().BuildMock();
            this.datasetRepository
                .Setup(d => d.All())
                .Returns(mockDatasets.Object);

            var mockMetaData = new List<MetaData>().AsQueryable().BuildMock();
            this.datasetRepository
                .Setup(d => d.SaveChangesAsync());

            this.stockService = new StockService(this.stockRepository.Object, this.intervalRepository.Object, this.datasetRepository.Object, this.metadataRepository.Object, this.memoryCache);
        }

        [Test]
        public async Task GetLastPriceReturnCorrectData()
        {
            var result = await this.stockService.GetLastPrice(GlobalConstants.StockTicker);

            StringAssert.Contains("100.00", result);
        }

        [Test]
        public async Task GetLastPriceReturnZeroForWrongTicker()
        {
            var result = await this.stockService.GetLastPrice("None");

            StringAssert.Contains("0.00", result);
        }

        [Test]
        public async Task GetLastUpdatedTimeReturnCorrectData()
        {
            var result = await this.stockService.GetLastUpdatedTime(GlobalConstants.StockTicker);

            var expectedDate = DateTime.Parse("2020-04-09 00:00:00").Date;

            Assert.AreEqual(expectedDate, result.Date);
        }

        [Test]
        public async Task GetLastUpdatedTimeReturnFirstDayForWrongTicker()
        {
            var result = await this.stockService.GetLastUpdatedTime("None");

            var expectedDate = DateTime.Parse("01.01.0001").Date;

            Assert.AreEqual(expectedDate, result.Date);
        }

        [Test]
        public async Task GetLastPriceAndTimeReturnCorrectData()
        {
            var result = await this.stockService.GetLastPriceAndTime(GlobalConstants.StockTicker);

            var expectedDate = DateTime.Parse("2020-04-09 00:00:00").ToString("g", CultureInfo.InvariantCulture);

            StringAssert.Contains(expectedDate, result.DateTime);
            StringAssert.Contains("100.00", result.Price);
        }

        [Test]
        public async Task GetLastPriceAndTimeReturnNullForWrongTicker()
        {
            var result = await this.stockService.GetLastPriceAndTime("None");
            Assert.Null(result);
        }

        [Test]
        public async Task GetUpdateWorkCorrectlyWithEmptyData()
        {
            var result = await this.stockService.GetUpdate(string.Empty, GlobalConstants.StockTicker);

            Assert.NotNull(result);
        }

        [Test]
        public async Task GetUpdateWorkCorrectly()
        {
            var result = await this.stockService.GetUpdate("09/04/2020 00:00", GlobalConstants.StockTicker);

            Assert.NotNull(result);
        }

        [Test]
        public async Task GetUpdateDoesNotWorkCorrectlyWithWrongTicker()
        {
            var result = await this.stockService.GetUpdate(string.Empty, "None");

            StringAssert.Contains("0.00", result.NewPrice);
        }

        [Test]
        public async Task ImportDataShouldNotUpdateMetadataRepositoryWithUpToDateData()
        {
            await this.stockService.ImportData(TestDataHelpers.GetUpToDateJSON(), GlobalConstants.StockTicker);

            this.metadataRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task ImportDataShouldUpdateRepositoriesWithNotUpdatedData()
        {
            await this.stockService.ImportData(TestDataHelpers.GetNotUpdatedJSON(), GlobalConstants.StockTicker);

            this.metadataRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
            this.datasetRepository.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
        }

        [Test]
        public async Task ImportDataShouldUpdateRepositoriesWithNewData()
        {
            await this.stockService.ImportData(TestDataHelpers.GetNewDateJSON(), GlobalConstants.StockTicker);

            this.metadataRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
            this.datasetRepository.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
        }

        [Test]
        public async Task CreateStockWorksCorrectly()
        {
            await this.stockService.CreateStockAsync(GlobalConstants.StockTicker, GlobalConstants.StockName, GlobalConstants.StockInterval);

            this.stockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetPricesLast300MinutesWithCorrectTickerReturnNotEmptyCollection()
        {
            var result = await this.stockService.GetPricesLast300Minutes(GlobalConstants.StockTicker);

            CollectionAssert.IsNotEmpty(result);
        }

        [Test]
        public async Task GetPricesLast300MinutesWithInCorrectTickerReturnEmptyCollection()
        {
            var result = await this.stockService.GetPricesLast300Minutes("None");

            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public async Task GetPricesLast300MinutesCallsDatasetRepository()
        {
            var result = await this.stockService.GetPricesLast300Minutes("None");

            this.datasetRepository.Verify(x => x.All(), Times.Once);
        }
    }
}
