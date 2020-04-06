namespace PersonalStockTrader.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Update;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Web.ViewModels.User.TradeHistory;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;
    using PersonalStockTrader.Web.ViewModels.User.TradeShares;

    public class PositionsService : IPositionsService
    {
        private const string Buy = "Bought";
        private const string Sell = "Sold";
        private static IDeletableEntityRepository<DataSet> datasetsRepository;

        private readonly IDeletableEntityRepository<Position> positionRepository;
        private readonly IDeletableEntityRepository<Account> accountRepository;
        private readonly IDeletableEntityRepository<Stock> stockRepository;

        public PositionsService(IDeletableEntityRepository<Position> positionRepository, IDeletableEntityRepository<Account> accountRepository, IDeletableEntityRepository<Stock> stockRepository, IDeletableEntityRepository<DataSet> repository)
        {
            this.positionRepository = positionRepository;
            this.accountRepository = accountRepository;
            this.stockRepository = stockRepository;
            datasetsRepository = repository;
        }

        public async Task<TradeSharesResultModel> OpenPosition(int accountId, int numberShares, bool isBuy)
        {
            var account = await this.accountRepository
                .All()
                .FirstOrDefaultAsync(a => a.Id == accountId);

            var stockId = await this.stockRepository
                .All()
                .Where(s => s.Ticker == GlobalConstants.StockTicker)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            var position = new Position
            {
                StockId = stockId,
                AccountId = accountId,
                CountStocks = numberShares,
                TypeOfTrade = isBuy ? TypeOfTrade.Buy : TypeOfTrade.Sell,
                OpenClose = OpenClose.Open,
            };

            await this.positionRepository.AddAsync(position);
            await this.positionRepository.SaveChangesAsync();

            account.Positions.Add(position);

            var positionStockPrice = FindPositionOpenPrice(position.CreatedOn);
            account.Balance -= position.CountStocks * positionStockPrice;

            this.accountRepository.Update(account);
            await this.accountRepository.SaveChangesAsync();

            var result =  new TradeSharesResultModel
            {
                PositionId = position.Id,
                Quantity = position.CountStocks,
                OpenPrice = FindPositionOpenPrice(position.CreatedOn),
                Balance = account.Balance,
                IsBuy = position.TypeOfTrade == TypeOfTrade.Buy ? true : false,
            };

            return result;
        }

        public async Task<TradeSharesResultModel> UpdatePosition(int accountId, int positionId, int numberShares, bool isBuy)
        {
            var position = await this.positionRepository
                .All()
                .FirstOrDefaultAsync(p => p.Id == positionId);

            var currentShares = position.CountStocks;
            var newPositionDirection = isBuy == true ? TypeOfTrade.Buy : TypeOfTrade.Sell;

            await this.ClosePosition(accountId);

            if (newPositionDirection == position.TypeOfTrade)
            {
                return await this.OpenPosition(accountId, numberShares + currentShares, isBuy);
            }
            else if (numberShares > position.CountStocks)
            {
                return await this.OpenPosition(accountId, numberShares - currentShares, isBuy);
            }
            else if (numberShares < position.CountStocks)
            {
                return await this.OpenPosition(accountId, currentShares - numberShares, !isBuy);
            }

            return new TradeSharesResultModel();
        }

        public async Task ClosePosition(int accountId)
        {
            var account = await this.accountRepository
                .All()
                .FirstOrDefaultAsync(a => a.Id == accountId);

            var position = account.Positions.FirstOrDefault(p => p.OpenClose == OpenClose.Open);
            var currentStockPrice = await GetCurrentStockPrice();

            if (position != null)
            {
                position.OpenClose = OpenClose.Close;
                account.Balance -= account.TradeFee;
                account.Balance += position.CountStocks * currentStockPrice;
                var tradeFee = new FeePayment
                {
                    Amount = account.TradeFee,
                    TypeFee = TypeFee.TradeFee,
                };

                account.Fees.Add(tradeFee);
                await this.accountRepository.SaveChangesAsync();
            }
        }

        public async Task<PositionViewModel> GetOpenPosition(int accountId)
        {
            var position = await this.positionRepository
                .All()
                .Where(p => p.AccountId == accountId && p.OpenClose == OpenClose.Open)
                .Select(p => new PositionViewModel
                {
                    PositionId = p.Id,
                    Quantity = p.CountStocks,
                    Direction = p.TypeOfTrade == TypeOfTrade.Buy,
                    OpenPrice = FindPositionOpenPrice(p.CreatedOn),
                })
                .FirstOrDefaultAsync();

            return position;
        }

        public async Task<IEnumerable<HistoryPositionViewModel>> GetAccountClosedPositions(int accountId, string startDate, string endDate)
        {
            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);

            var result = await this.positionRepository
                .All()
                .Where(p => p.AccountId == accountId && 
                            p.OpenClose == OpenClose.Close &&
                            p.CreatedOn >= start && 
                            p.CreatedOn <= end)
                .Select(p => new HistoryPositionViewModel
                {
                    Ticker = GlobalConstants.StockTicker,
                    OpenDate = p.CreatedOn,
                    Quantity = p.CountStocks,
                    Direction = p.TypeOfTrade == TypeOfTrade.Buy ? Buy : Sell,
                    OpenPrice = FindPositionOpenPrice(p.CreatedOn),
                    ClosePrice = FindPositionOpenPrice((DateTime)p.ModifiedOn),
                    Profit = p.TypeOfTrade == TypeOfTrade.Buy ?
                        p.CountStocks * (FindPositionOpenPrice((DateTime)p.ModifiedOn) - FindPositionOpenPrice(p.CreatedOn)) :
                        p.CountStocks * (FindPositionOpenPrice(p.CreatedOn) - FindPositionOpenPrice((DateTime)p.ModifiedOn)),
                })
                .ToListAsync();

            return result;
        }

        private static async Task<decimal> GetCurrentStockPrice()
        {
            return await datasetsRepository
                .All()
                .OrderByDescending(d => d.DateAndTime)
                .Select(d => d.ClosePrice)
                .FirstOrDefaultAsync();
        }

        private static decimal FindPositionOpenPrice(DateTime openTime)
        {
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(openTime, easternZone);

            var price =  datasetsRepository
                .All()
                .OrderByDescending(d => d.DateAndTime)
                .Where(d => d.DateAndTime <= easternTime)
                .Select(d => d.ClosePrice)
                .FirstOrDefault();

            return price;
        }
    }
}
