namespace PersonalStockTrader.Services.Data.Tests.ServiceTests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Web.ViewModels.User.TradeHistory;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;

    public static class TestDataHelpers
    {
        public static List<Account> GetTestData()
        {
            var users = GetTestUsers();
            var positions = GetTestPositions();
            var fees = GetTestFeePayments();

            return new List<Account>()
            {
                new Account
                {
                    Id = 1,
                    UserId = "1",
                    User = users[0],
                    Balance = 2000M,
                    MonthlyFee = 50,
                    Positions = positions.Where(p => p.AccountId == 1).ToList(),
                    Fees = fees.Where(f => f.AccountId == 1).ToList(),
                },
                new Account
                {
                    Id = 2,
                    UserId = "2",
                    User = users[1],
                    Balance = 5000M,
                    MonthlyFee = 50,
                    Positions = positions.Where(p => p.AccountId == 2).ToList(),
                    Fees = fees.Where(f => f.AccountId == 2).ToList(),
                },
            };
        }

        public static List<ApplicationUser> GetTestUsers()
        {
            return new List<ApplicationUser>()
            {
                new ApplicationUser
                {
                    Id = "1",
                    UserName = "A",
                    Email = "a@a.a",
                    StartBalance = 2000.00M,
                    IsDeleted = false,
                },
                new ApplicationUser
                {
                    Id = "2",
                    UserName = "B",
                    Email = "b@b.b",
                    StartBalance = 3000.00M,
                    IsDeleted = false,
                },
            };
        }

        public static List<ApplicationUser> GetTestNotConfirmedUsers()
        {
            return new List<ApplicationUser>()
            {
                new ApplicationUser
                {
                    Id = "11",
                    UserName = "E",
                    Email = "e@e.e",
                    StartBalance = 2000.00M,
                    IsDeleted = false,
                },
                new ApplicationUser
                {
                    Id = "22",
                    UserName = "F",
                    Email = "f@f.f",
                    StartBalance = 3000.00M,
                    IsDeleted = false,
                },
            };
        }

        public static List<Position> GetTestPositions()
        {
            return new List<Position>()
            {
                new Position
                {
                    Id = 1,
                    CreatedOn = DateTime.Parse("01.05.2019"),
                    ModifiedOn = DateTime.Parse("01.05.2019"),
                    AccountId = 1,
                    OpenClose = OpenClose.Close,
                    TypeOfTrade = TypeOfTrade.Buy,
                },
                new Position
                {
                    Id = 2,
                    CreatedOn = DateTime.Parse("01.03.2020"),
                    ModifiedOn = DateTime.Parse("01.03.2020"),
                    AccountId = 1,
                    OpenClose = OpenClose.Close,
                    TypeOfTrade = TypeOfTrade.Buy,
                },
                new Position
                {
                    Id = 3,
                    CreatedOn = DateTime.Parse("01.04.2020"),
                    AccountId = 1,
                    CountStocks = 10,
                    OpenClose = OpenClose.Open,
                    TypeOfTrade = TypeOfTrade.Buy,
                },
                new Position
                {
                    Id = 4,
                    CreatedOn = DateTime.Parse("01.05.2019"),
                    ModifiedOn = DateTime.Parse("01.05.2019"),
                    AccountId = 2,
                    OpenClose = OpenClose.Close,
                    TypeOfTrade = TypeOfTrade.Buy,
                },
                new Position
                {
                    Id = 5,
                    CreatedOn = DateTime.Parse("01.03.2020"),
                    ModifiedOn = DateTime.Parse("01.03.2020"),
                    AccountId = 2,
                    OpenClose = OpenClose.Close,
                    TypeOfTrade = TypeOfTrade.Buy,
                },
                new Position
                {
                    Id = 6,
                    CreatedOn = DateTime.Parse("01.04.2020"),
                    AccountId = 2,
                    OpenClose = OpenClose.Open,
                    TypeOfTrade = TypeOfTrade.Buy,
                },
            };
        }

        public static IEnumerable<HistoryPositionViewModel> GetTestPositionsTradeHistory()
        {
            return new List<HistoryPositionViewModel>()
            {
                new HistoryPositionViewModel
                {
                    Ticker = null,
                    OpenDate = default,
                    Quantity = 0,
                    Direction = null,
                    OpenPrice = 0,
                    ClosePrice = 0,
                    Profit = 0,
                },
                new HistoryPositionViewModel
                {
                    Ticker = null,
                    OpenDate = default,
                    Quantity = 0,
                    Direction = null,
                    OpenPrice = 0,
                    ClosePrice = 0,
                    Profit = 0,
                },
            };
        }

        public static PositionViewModel GetTestPosition()
        {
            return new PositionViewModel
            {
                PositionId = 1,
                Quantity = 10,
                Direction = false,
                OpenPrice = 100.00M,
            };
        }

        public static List<HistoryPositionViewModel> GetTestHistoryPositions()
        {
            return new List<HistoryPositionViewModel>()
            {
                new HistoryPositionViewModel
                {
                    Ticker = GlobalConstants.StockTicker,
                    OpenDate = DateTime.Parse("09.04.2020"),
                    Quantity = 10,
                    Direction = "Buy",
                    OpenPrice = 100.00M,
                    ClosePrice = 101.00M,
                    Profit = 10.00M,
                },
                new HistoryPositionViewModel
                {
                    Ticker = GlobalConstants.StockTicker,
                    OpenDate = DateTime.Parse("08.04.2020"),
                    Quantity = 20,
                    Direction = "Buy",
                    OpenPrice = 100.00M,
                    ClosePrice = 101.00M,
                    Profit = 20.00M,
                },
            };
        }

        public static List<DataSet> GetTestDataSets()
        {
            return new List<DataSet>()
            {
                new DataSet()
                {
                    DateAndTime = DateTime.Parse("09.04.2020"),
                    ClosePrice = 100.00M,
                    IntervalId = 1,
                },
                new DataSet()
                {
                    DateAndTime = DateTime.Parse("31.12.2018"),
                    ClosePrice = 100.00M,
                    IntervalId = 1,
                },
            };
        }

        public static List<Interval> GetTestIntervals()
        {
            return new List<Interval>()
            {
                new Interval()
                {
                    Id = 1,
                    StockId = 1,
                },
            };
        }

        public static List<Stock> GetTestStocks()
        {
            return new List<Stock>()
            {
                new Stock()
                {
                    Id = 1,
                    Ticker = GlobalConstants.StockTicker,
                },
            };
        }

        public static List<FeePayment> GetTestFeePayments()
        {
            return new List<FeePayment>()
            {
                new FeePayment
                {
                    Id = 1,
                    CreatedOn = DateTime.Parse("09.04.2020"),
                    Amount = 50,
                    TypeFee = TypeFee.TradeFee,
                    AccountId = 1,
                },
                new FeePayment
                {
                    Id = 2,
                    CreatedOn = DateTime.Parse("08.04.2020"),
                    Amount = 50,
                    TypeFee = TypeFee.TradeFee,
                    AccountId = 1,
                },
                new FeePayment
                {
                    Id = 3,
                    CreatedOn = DateTime.Parse("09.04.2020"),
                    Amount = 100,
                    TypeFee = TypeFee.MonthlyCommission,
                    AccountId = 1,
                },
                new FeePayment
                {
                    Id = 4,
                    CreatedOn = DateTime.Parse("09.04.2020"),
                    Amount = 50,
                    TypeFee = TypeFee.TradeFee,
                    AccountId = 2,
                },
                new FeePayment
                {
                    Id = 5,
                    CreatedOn = DateTime.Parse("08.04.2020"),
                    Amount = 50,
                    TypeFee = TypeFee.TradeFee,
                    AccountId = 2,
                },
                new FeePayment
                {
                    Id = 6,
                    CreatedOn = DateTime.Parse("09.04.2020"),
                    Amount = 100,
                    TypeFee = TypeFee.MonthlyCommission,
                    AccountId = 2,
                },
            };
        }

        public static string GetUpToDateJSON()
        {
            return "{\"Meta Data\": {\"1. Information\": \"Intraday (1min) open, high, low, close prices and volume\",\"2. Symbol\": \"IBM\",\"3. Last Refreshed\": \"2020-04-09 00:00:00\",\"4. Interval\": \"1min\",\"5. Output Size\": \"Compact\",\"6. Time Zone\": \"US/Eastern\"},\"Time Series (1min)\": {\"2020-04-09 00:00:00\": {\"1. open\": \"122.1400\",\"2. high\": \"122.2100\",\"3. low\": \"121.4300\",\"4. close\": \"121.5200\",\"5. volume\": \"269094\"},\"2020-04-08 23:59:00\": {\"1. open\": \"121.6500\",\"2. high\": \"122.2300\",\"3. low\": \"121.2879\",\"4. close\": \"122.1350\",\"5. volume\": \"226196\"}}}";
        }

        public static string GetNewDateJSON()
        {
            return "{\"Meta Data\": {\"1. Information\": \"Intraday (1min) open, high, low, close prices and volume\",\"2. Symbol\": \"IBM\",\"3. Last Refreshed\": \"2020-04-10 00:00:00\",\"4. Interval\": \"1min\",\"5. Output Size\": \"Compact\",\"6. Time Zone\": \"US/Eastern\"},\"Time Series (1min)\": {\"2020-04-10 00:00:00\": {\"1. open\": \"122.1400\",\"2. high\": \"122.2100\",\"3. low\": \"121.4300\",\"4. close\": \"121.5200\",\"5. volume\": \"269094\"},\"2020-04-09 23:59:00\": {\"1. open\": \"121.6500\",\"2. high\": \"122.2300\",\"3. low\": \"121.2879\",\"4. close\": \"122.1350\",\"5. volume\": \"226196\"}}}";
        }

        public static string GetNotUpdatedJSON()
        {
            return "{\"Meta Data\": {\"1. Information\": \"Intraday (1min) open, high, low, close prices and volume\",\"2. Symbol\": \"IBM\",\"3. Last Refreshed\": \"2020-03-09 16:00:00\",\"4. Interval\": \"1min\",\"5. Output Size\": \"Compact\",\"6. Time Zone\": \"US/Eastern\"},\"Time Series (1min)\": {\"2020-03-09 16:00:00\": {\"1. open\": \"122.1400\",\"2. high\": \"122.2100\",\"3. low\": \"121.4300\",\"4. close\": \"121.5200\",\"5. volume\": \"269094\"},\"2020-03-09 15:59:00\": {\"1. open\": \"121.6500\",\"2. high\": \"122.2300\",\"3. low\": \"121.2879\",\"4. close\": \"122.1350\",\"5. volume\": \"226196\"}}}";
        }
    }
}
