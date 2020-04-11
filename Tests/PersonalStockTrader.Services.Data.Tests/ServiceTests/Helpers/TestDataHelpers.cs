namespace PersonalStockTrader.Services.Data.Tests.ServiceTests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;
    using Web.ViewModels.User.TradeHistory;

    public static class TestDataHelpers
    {
        public static List<Account> GetTestData()
        {
            var users = GetTestUsers();
            var positions = GetTestPositions();

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
                },
                new Account
                {
                    Id = 2,
                    UserId = "2",
                    User = users[1],
                    Balance = 5000M,
                    MonthlyFee = 50,
                    Positions = positions.Where(p => p.AccountId == 2).ToList(),
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
                },
                new ApplicationUser
                {
                    Id = "2",
                    UserName = "B",
                    Email = "b@b.b",
                    StartBalance = 3000.00M,
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
                    AccountId = 1,
                    OpenClose = OpenClose.Close,
                },
                new Position
                {
                    Id = 2,
                    CreatedOn = DateTime.Parse("01.03.2020"),
                    AccountId = 1,
                    OpenClose = OpenClose.Close,
                },
                new Position
                {
                    Id = 2,
                    CreatedOn = DateTime.Parse("01.04.2020"),
                    AccountId = 1,
                    OpenClose = OpenClose.Open,
                },
                new Position
                {
                    Id = 3,
                    CreatedOn = DateTime.Parse("01.05.2019"),
                    AccountId = 2,
                    OpenClose = OpenClose.Close,
                },
                new Position
                {
                    Id = 4,
                    CreatedOn = DateTime.Parse("01.03.2020"),
                    AccountId = 2,
                    OpenClose = OpenClose.Close,
                },
                new Position
                {
                    Id = 4,
                    CreatedOn = DateTime.Parse("01.04.2020"),
                    AccountId = 2,
                    OpenClose = OpenClose.Open,
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
    }
}