namespace PersonalStockTrader.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;
    using PersonalStockTrader.Web.ViewModels.User.TradeShares;

    public class AccountService : IAccountService
    {
        private readonly IDeletableEntityRepository<Account> accountRepository;
        private readonly IPositionsService positionsService;

        public AccountService(IDeletableEntityRepository<Account> accountRepository, IPositionsService positionsService)
        {
            this.accountRepository = accountRepository;
            this.positionsService = positionsService;
        }

        public async Task<TradeSharesResultModel> ManagePositions(TradeSharesInputViewModel input)
        {
            var result = new TradeSharesResultModel();

            if (int.Parse(input.PositionId) != 0)
            {
                result = await this.positionsService.UpdatePosition(int.Parse(input.AccountId), int.Parse(input.PositionId), int.Parse(input.Quantity), input.IsBuy);
            }
            else
            {
                result = await this.positionsService.OpenPosition(int.Parse(input.AccountId), int.Parse(input.Quantity), input.IsBuy);
            }

            return result;
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
    }
}