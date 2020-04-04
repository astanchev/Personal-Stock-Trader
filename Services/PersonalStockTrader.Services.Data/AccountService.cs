namespace PersonalStockTrader.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;

    public class AccountService : IAccountService
    {
        private readonly IDeletableEntityRepository<Account> accountRepository;
        private readonly IPositionsService positionsService;

        public AccountService(IDeletableEntityRepository<Account> accountRepository, IPositionsService positionsService)
        {
            this.accountRepository = accountRepository;
            this.positionsService = positionsService;
        }

        public async Task ManagePositions(TradeSharesInputViewModel input)
        {
            if (input.PositionId != 0)
            {
                await this.positionsService.UpdatePosition(input.AccountId, input.PositionId, input.Quantity, input.IsBuy);
            }
            else
            {
                await this.positionsService.OpenPosition(input.AccountId, input.Quantity, input.IsBuy);
            }
        }

        public async Task<PositionViewModel> GetCurrentPosition(int accountId)
        {
            var account = await this.accountRepository
                .All()
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account.Positions.All(p => p.OpenClose == OpenClose.Close))
            {
                return new PositionViewModel();
            }
            else
            {
                return await this.positionsService.GetOpenPosition(accountId);
            }
        }
    }
}