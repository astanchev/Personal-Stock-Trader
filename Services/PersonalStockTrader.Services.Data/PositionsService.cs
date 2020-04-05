namespace PersonalStockTrader.Services.Data
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Update;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;

    public class PositionsService : IPositionsService
    {
        private readonly IDeletableEntityRepository<Position> positionRepository;
        private readonly IDeletableEntityRepository<Account> accountRepository;
        private readonly IDeletableEntityRepository<Stock> stockRepository;
        private static IDeletableEntityRepository<DataSet> datasetsRepository;

        public PositionsService(IDeletableEntityRepository<Position> positionRepository, IDeletableEntityRepository<Account> accountRepository, IDeletableEntityRepository<Stock> stockRepository, IDeletableEntityRepository<DataSet> repository)
        {
            this.positionRepository = positionRepository;
            this.accountRepository = accountRepository;
            this.stockRepository = stockRepository;
            datasetsRepository = repository;
        }

        public async Task OpenPosition(int accountId, int numberShares, bool isBuy)
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
        }

        public async Task UpdatePosition(int accountId, int positionId, int numberShares, bool isBuy)
        {
            var position = await this.positionRepository
                .All()
                .FirstOrDefaultAsync(p => p.Id == positionId);
            var account = await this.accountRepository
                .All()
                .FirstOrDefaultAsync(a => a.Id == accountId);
            var currentStockPrice = await datasetsRepository
                .All()
                .OrderByDescending(d => d.DateAndTime)
                .Select(d => d.ClosePrice)
                .FirstOrDefaultAsync();
            var currentShares = position.CountStocks;
            account.Balance += currentShares * currentStockPrice;

            var newPositionDirection = isBuy == true ? TypeOfTrade.Buy : TypeOfTrade.Sell;

            if (newPositionDirection == position.TypeOfTrade)
            {
                position.CountStocks += numberShares;
                account.Balance -= (numberShares + currentShares) * currentStockPrice;
            }
            else if (numberShares > position.CountStocks)
            {
                position.CountStocks = numberShares - position.CountStocks;
                position.TypeOfTrade = newPositionDirection;
                account.Balance -= (numberShares - position.CountStocks) * currentStockPrice;
            }
            else if (numberShares < position.CountStocks)
            {
                position.CountStocks = position.CountStocks - numberShares;
                account.Balance -= (position.CountStocks - numberShares) * currentStockPrice;
            }
            else
            {
                position.CountStocks = 0;
                position.OpenClose = OpenClose.Close;
                account.Balance -= account.TradeFee;
                var tradeFee = new FeePayment
                {
                    Amount = account.TradeFee,
                    TypeFee = TypeFee.TradeFee,
                };
                account.Fees.Add(tradeFee);
            }

            await this.positionRepository.SaveChangesAsync();
            await this.accountRepository.SaveChangesAsync();
        }

        public async Task ClosePosition(int accountId)
        {
            var account = await this.accountRepository
                .All()
                .FirstOrDefaultAsync(a => a.Id == accountId);

            var position = account.Positions.FirstOrDefault(p => p.OpenClose == OpenClose.Open);

            if (position != null)
            {
                position.CountStocks = 0;
                position.OpenClose = OpenClose.Close;
                account.Balance -= account.TradeFee;
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
                    OpenPrice = FindPositionOpenPrice(p.ModifiedOn ?? p.CreatedOn),
                })
                .FirstOrDefaultAsync();

            return position;
        }

        private static decimal FindPositionOpenPrice(DateTime openTime)
        {
            return datasetsRepository
                .All()
                .OrderByDescending(d => d.DateAndTime)
                .Where(d => d.DateAndTime <= openTime)
                .Select(d => d.ClosePrice)
                .FirstOrDefault();
        }
    }
}
