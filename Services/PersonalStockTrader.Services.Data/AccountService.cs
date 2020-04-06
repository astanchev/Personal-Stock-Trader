namespace PersonalStockTrader.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.EntityFrameworkCore;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;
    using PersonalStockTrader.Web.ViewModels.User.TradeShares;
    using PersonalStockTrader.Web.ViewModels.User.TradeHistory;

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

        public async Task<TradeSharesResultModel> ManagePositions(TradeSharesInputViewModel input)
        {
            if (int.Parse(input.PositionId) != 0)
            {
                return await this.positionsService.UpdatePosition(int.Parse(input.AccountId), int.Parse(input.PositionId), int.Parse(input.Quantity), input.IsBuy);
            }
            else
            {
                return await this.positionsService.OpenPosition(int.Parse(input.AccountId), int.Parse(input.Quantity), input.IsBuy);
            }
        }

        public async Task<PositionViewModel> GetCurrentPosition(int accountId)
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

        public Task<TradeHistoryViewModel> GetAllClosedPositionsByUserId(string userId)
        {
            var startDate = this.archiveStartDate.ToShortDateString();
            var endDate = DateTime.UtcNow.ToShortDateString();

            return this.GetAllClosedPositionsIntervalByUserId(userId, startDate, endDate);
        }

        public async Task<TradeHistoryViewModel> GetAllClosedPositionsIntervalByUserId(string userId, string startDate, string endDate)
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

            var closedPositions = await this.positionsService.GetAccountClosedPositions(accountResult.AccountId, startDate, endDate);

            var result = new TradeHistoryViewModel
            {
                Username = accountResult.Username,
                Email = accountResult.Email,
                AccountId = accountResult.AccountId,
                StartBalance = accountResult.StartBalance,
                Balance = accountResult.Balance,
                Positions = closedPositions,
            };

            return result;
        }
    }
}