namespace PersonalStockTrader.Services.Data.Tests.ServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using MockQueryable.Moq;
    using Moq;
    using NUnit.Framework;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Data.Repositories;
    using PersonalStockTrader.Services.Data.Tests.ServiceTests.Helpers;

    [TestFixture]
    public class PositionsServiceTests
    {
        private Mock<IDeletableEntityRepository<Account>> accountRepository;
        private IDeletableEntityRepository<Position> positionRepository;
        private Mock<IDeletableEntityRepository<Stock>> stockRepository;
        private Mock<IDeletableEntityRepository<DataSet>> datasetRepository;
        private Mock<IQueryable<Account>> mockAccounts;
        private Mock<IQueryable<Stock>> mockStocks;
        private PositionsService positionsService;

        [SetUp]
        public void Setup()
        {
            var context = ApplicationDbContextInMemoryFactory.InitializeContext();
            this.positionRepository = new EfDeletableEntityRepository<Position>(context);

            this.mockAccounts = TestDataHelpers.GetTestData().AsQueryable().BuildMock();
            this.mockStocks = TestDataHelpers.GetTestStocks().AsQueryable().BuildMock();

            this.accountRepository = new Mock<IDeletableEntityRepository<Account>>();
            this.stockRepository = new Mock<IDeletableEntityRepository<Stock>>();
            this.datasetRepository = new Mock<IDeletableEntityRepository<DataSet>>();

            this.accountRepository
                .Setup(a => a.All())
                .Returns(this.mockAccounts.Object);

            this.stockRepository
                .Setup(s => s.All())
                .Returns(this.mockStocks.Object);

            this.datasetRepository
                .Setup(d => d.All())
                .Returns(new List<DataSet>()
                {
                    new DataSet()
                    {
                        DateAndTime = DateTime.Now,
                        ClosePrice = 100.00M,
                    },
                    new DataSet()
                    {
                        DateAndTime = DateTime.Parse("31.12.2018"),
                        ClosePrice = 100.00M,
                    },
                }.AsQueryable());

            this.positionsService = new PositionsService(this.positionRepository, this.accountRepository.Object, this.stockRepository.Object, this.datasetRepository.Object);
        }

        [Test]
        public async Task OpenPositionShouldCreatePosition()
        {
            var countPreAdd = this.positionRepository.All().Count();
            await this.positionsService.OpenPosition(1, 10, true);
            var countAfterAdd = this.positionRepository.All().Count();

            Assert.AreEqual(countPreAdd + 1, countAfterAdd);
        }

        [Test]
        public async Task OpenPositionReturnsCorrectResult()
        {
            var result = await this.positionsService.OpenPosition(1, 10, true);

            Assert.AreEqual(10, result.Quantity);
            Assert.AreEqual(true, result.IsBuy);
        }

        [Test]
        public async Task OpenPositionShouldCreatePositionForCorrectAccountId()
        {
            var countAccPossPre = this.accountRepository
                .Object
                .All()
                .Where(a => a.Id == 1)
                .Select(a => a.Positions.Count)
                .FirstOrDefault();

            var result = await this.positionsService.OpenPosition(1, 10, true);

            var countAccPossAfter = this.accountRepository
                .Object
                .All()
                .Where(a => a.Id == 1)
                .Select(a => a.Positions.Count)
                .FirstOrDefault();

            Assert.AreEqual(countAccPossPre + 1, countAccPossAfter);
        }

        [Test]
        public async Task OpenPositionDirectCorrectRepositories()
        {
            await this.positionsService.OpenPosition(1, 10, true);

            this.accountRepository.Verify(a => a.All(), Times.Once);
            this.stockRepository.Verify(s => s.All(), Times.Once);
        }

        [Test]
        public async Task ClosePositionShouldWorkCorrectly()
        {
            var position = this.accountRepository
                .Object
                .All()
                .Where(a => a.Id == 1)
                .Select(a => a.Positions
                    .FirstOrDefault(p => p.OpenClose == OpenClose.Open))
                .FirstOrDefault();

            Assert.NotNull(position);
            await this.positionsService.ClosePosition(1);
            var closedPosition = this.accountRepository
                .Object
                .All()
                .Where(a => a.Id == 1)
                .Select(a => a.Positions
                    .FirstOrDefault(p => p.Id == position.Id))
                .FirstOrDefault();

            Assert.NotNull(closedPosition);
            Assert.AreEqual(OpenClose.Close, closedPosition.OpenClose);
        }

        [Test]
        public async Task ClosePositionDirectDataSetRepository()
        {
            await this.positionsService.ClosePosition(1);

            this.datasetRepository.Verify(a => a.All(), Times.Once);
        }

        [Test]
        public async Task ClosePositionDirectAccountRepository()
        {
            await this.positionsService.ClosePosition(1);

            this.accountRepository.Verify(a => a.All(), Times.AtLeastOnce);
        }

        [Test]
        public async Task UpdatePositionFirstShouldClosePosition()
        {
            var position = this.accountRepository
                .Object
                .All()
                .Where(a => a.Id == 1)
                .Select(a => a.Positions
                    .FirstOrDefault(p => p.Id == 3))
                .FirstOrDefault();

            Assert.NotNull(position);
            Assert.AreEqual(OpenClose.Open, position.OpenClose);

            var mockPositions = new List<Position>() {position}.AsQueryable().BuildMock();
            var testPositionRepository = new Mock<IDeletableEntityRepository<Position>>();
            testPositionRepository
                .Setup(x => x.All())
                .Returns(mockPositions.Object);
            var testPositionService = new PositionsService(testPositionRepository.Object, this.accountRepository.Object, this.stockRepository.Object, this.datasetRepository.Object);

            await testPositionService.UpdatePosition(1, 3, 10, true);
            var closedPosition = this.accountRepository
                .Object
                .All()
                .Where(a => a.Id == 1)
                .Select(a => a.Positions
                    .FirstOrDefault(p => p.Id == 3))
                .FirstOrDefault();

            Assert.NotNull(closedPosition);
            Assert.AreEqual(OpenClose.Close, closedPosition.OpenClose);
        }

        [Test]
        public async Task UpdatePositionShouldWorkCorrectly()
        {
            var position = this.accountRepository
                .Object
                .All()
                .Where(a => a.Id == 1)
                .Select(a => a.Positions
                    .FirstOrDefault(p => p.Id == 3))
                .FirstOrDefault();

            Assert.NotNull(position);
            Assert.AreEqual(OpenClose.Open, position.OpenClose);

            var mockPositions = new List<Position>() {position}.AsQueryable().BuildMock();
            var testPositionRepository = new Mock<IDeletableEntityRepository<Position>>();
            testPositionRepository
                .Setup(x => x.All())
                .Returns(mockPositions.Object);
            var testPositionService = new PositionsService(testPositionRepository.Object, this.accountRepository.Object, this.stockRepository.Object, this.datasetRepository.Object);

            var result = await testPositionService.UpdatePosition(1, 3, 10, true);

            Assert.NotNull(result);
        }

        [Test]
        public async Task UpdatePositionCallsOpenPosition()
        {
            var position = this.accountRepository
                .Object
                .All()
                .Where(a => a.Id == 1)
                .Select(a => a.Positions
                    .FirstOrDefault(p => p.Id == 3))
                .FirstOrDefault();

            Assert.NotNull(position);
            Assert.AreEqual(OpenClose.Open, position.OpenClose);

            var mockPositions = new List<Position>() {position}.AsQueryable().BuildMock();
            var testPositionRepository = new Mock<IDeletableEntityRepository<Position>>();
            testPositionRepository
                .Setup(x => x.All())
                .Returns(mockPositions.Object);
            var testPositionService = new PositionsService(testPositionRepository.Object, this.accountRepository.Object, this.stockRepository.Object, this.datasetRepository.Object);

            await testPositionService.UpdatePosition(1, 3, 10, true);

            this.accountRepository.Verify(a => a.All(), Times.AtLeastOnce);
            this.stockRepository.Verify(s => s.All(), Times.Once);
        }

        [Test]
        public async Task GetOpenPositionWithCorrectAccountIdReturnsCorrectData()
        {
            await this.positionsService.OpenPosition(1, 10, true);

            var result = await this.positionsService.GetOpenPosition(1);

            Assert.NotNull(result);
        }

        [Test]
        public async Task GetOpenPositionWithInCorrectAccountIdReturnsNull()
        {
            var result = await this.positionsService.GetOpenPosition(-1);

            Assert.Null(result);
        }

        [Test]
        public async Task GetAccountClosedPositionsWorkCorrectly()
        {
            var positions = TestDataHelpers.GetTestPositions();

            foreach (var position in positions)
            {
                await this.positionRepository.AddAsync(position);
                await this.positionRepository.SaveChangesAsync();
            }

            var result =
                await this.positionsService.GetAccountClosedPositions(1, DateTime.Parse("01.01.2019").ToShortDateString(), DateTime.Parse("01.01.2020").ToShortDateString());

            CollectionAssert.IsNotEmpty(result);
        }

        [Test]
        public async Task GetAccountClosedPositionsWithIncorrectDataReturnsEmptyCollection()
        {
            var result =
                await this.positionsService.GetAccountClosedPositions(-1, DateTime.Parse("01.01.2020").ToShortDateString(), DateTime.Parse("01.01.2019").ToShortDateString());

            CollectionAssert.IsEmpty(result);
        }
    }
}
