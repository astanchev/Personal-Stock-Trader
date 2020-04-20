namespace PersonalStockTrader.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Web.ViewModels.User.TradeHistory;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;
    using PersonalStockTrader.Web.ViewModels.User.TradeShares;

    public class AccountService : IAccountService
    {
        private readonly IDeletableEntityRepository<Account> accountRepository;
        private readonly DateTime archiveStartDate = DateTime.Parse("01/01/2020");
        private readonly IPositionsService positionsService;

        public AccountService(IDeletableEntityRepository<Account> accountRepository, IPositionsService positionsService)
        {
            this.accountRepository = accountRepository;
            this.positionsService = positionsService;
        }

        public async Task TakeAllAccountsMonthlyFeesAsync()
        {
            var openAccounts = await this.accountRepository
                .All()
                .Where(a => !a.IsDeleted)
                .ToListAsync();

            foreach (var account in openAccounts)
            {
                var tradeFee = new FeePayment
                {
                    Amount = account.MonthlyFee,
                    TypeFee = TypeFee.MonthlyCommission,
                };
                account.Balance -= tradeFee.Amount;
                account.Fees.Add(tradeFee);
                await this.accountRepository.SaveChangesAsync();
            }
        }

        public async Task<TradeSharesResultModel> ManagePositionsAsync(TradeSharesInputViewModel input)
        {
            if (int.Parse(input.PositionId) > 0)
            {
                return await this.positionsService.UpdatePosition(int.Parse(input.AccountId), int.Parse(input.PositionId), int.Parse(input.Quantity), input.IsBuy);
            }
            else if (int.Parse(input.PositionId) == 0)
            {
                return await this.positionsService.OpenPosition(int.Parse(input.AccountId), int.Parse(input.Quantity), input.IsBuy);
            }
            else
            {
                return null;
            }
        }

        public async Task<PositionViewModel> GetCurrentPositionAsync(int accountId)
        {
            var position = await this.positionsService.GetOpenPosition(accountId);

            if (position == null)
            {
                return new PositionViewModel();
            }
            else
            {
                return position;
            }
        }

        public Task<TradeHistoryViewModel> GetAllClosedPositionsByUserIdAsync(string userId)
        {
            var startDate = this.archiveStartDate.ToShortDateString();
            var endDate = DateTime.UtcNow.ToShortDateString();

            return this.GetAllClosedPositionsIntervalByUserIdAsync(userId, startDate, endDate);
        }

        public async Task<TradeHistoryViewModel> GetAllClosedPositionsIntervalByUserIdAsync(string userId, string startDate, string endDate)
        {
            var accountResult = await this.accountRepository
                .All()
                .Where(a => a.UserId == userId)
                .Select(a => new
                {
                    Username = a.User.UserName,
                    Email = a.User.Email,
                    AccountId = a.Id,
                    StartBalance = a.User.StartBalance,
                    Balance = a.Balance,
                })
                .FirstOrDefaultAsync();

            int accountId = accountResult.AccountId;

            var closedPositions = await this.positionsService.GetAccountClosedPositions(accountId, startDate, endDate);

            var result = new TradeHistoryViewModel
            {
                Username = accountResult.Username,
                Email = accountResult.Email,
                AccountId = accountResult.AccountId,
                StartBalance = accountResult.StartBalance,
                Balance = accountResult.Balance,
                Positions = closedPositions,
                StartDate = DateTime.Parse(startDate),
            };

            return result;
        }
    }
}
