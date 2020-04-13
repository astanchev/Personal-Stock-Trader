namespace PersonalStockTrader.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;

    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<Account> accountRepository;
        private readonly IPositionsService positionsService;

        public UserService(IDeletableEntityRepository<Account> accountRepository, IPositionsService positionsService)
        {
            this.accountRepository = accountRepository;
            this.positionsService = positionsService;
        }

        public async Task<decimal> GetUserBalanceAsync(string userId)
        {
            return await this.accountRepository
                .All()
                .Where(a => a.UserId == userId)
                .Select(a => a.Balance)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetUserAccountIdAsync(string userId)
        {
            return await this.accountRepository
                .All()
                .Where(a => a.UserId == userId)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<IDictionary<DateTime, decimal>> GetUserPaidTradeFees(string userId, string startDate, string endDate)
        {
            var result = this.CreateEmptyResult(startDate, endDate);
            var tradeFees = await this.accountRepository
                .All()
                .Where(a => a.UserId == userId)
                .Select(a => a.Fees
                    .Where(f => f.TypeFee == TypeFee.TradeFee)
                    .Select(f => new
                    {
                        Date = f.CreatedOn.Date,
                        Amount = f.Amount,
                    })
                )
                .FirstOrDefaultAsync();

            foreach (var tradeFee in tradeFees)
            {
                if (result.Keys.Contains(tradeFee.Date))
                {
                    result[tradeFee.Date] += tradeFee.Amount;
                }
            }

            return result;
        }

        public async Task<IDictionary<DateTime, decimal>> GetUserPaidMonthlyFees(string userId, string startDate, string endDate)
        {
            var result = this.CreateEmptyResult(startDate, endDate);
            var tradeFees = await this.accountRepository
                .All()
                .Where(a => a.UserId == userId)
                .Select(a => a.Fees
                    .Where(f => f.TypeFee == TypeFee.MonthlyCommission)
                    .Select(f => new
                    {
                        Date = f.CreatedOn.Date,
                        Amount = f.Amount,
                    })
                )
                .FirstOrDefaultAsync();

            foreach (var tradeFee in tradeFees)
            {
                if (result.Keys.Contains(tradeFee.Date))
                {
                    result[tradeFee.Date] += tradeFee.Amount;
                }
            }

            return result;
        }

        public async Task<IDictionary<DateTime, decimal>> GetUserProfitLoss(string userId, string startDate, string endDate)
        {
            var accountId = await this.GetUserAccountIdAsync(userId);
            var result = this.CreateEmptyResult(startDate, endDate);
            var positions = await this.positionsService.GetAccountClosedPositions(accountId, startDate, endDate);

            foreach (var position in positions)
            {
                if (result.Keys.Contains(position.OpenDate.Date))
                {
                    result[position.OpenDate.Date] += position.Profit;
                }
            }

            return result;
        }

        private IDictionary<DateTime, decimal> CreateEmptyResult(string startDate, string endDate)
        {
            var result = new Dictionary<DateTime, decimal>();

            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);

            for (DateTime i = start; i <= end; i = i.AddDays(1))
            {
                result.Add(i.Date, 0M);
            }

            return result;
        }
    }
}
