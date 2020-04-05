namespace PersonalStockTrader.Services.Data
{
    using System.Threading.Tasks;

    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;
    using PersonalStockTrader.Web.ViewModels.User.TradeShares;

    public interface IAccountService
    {
        Task<TradeSharesResultModel> ManagePositions(TradeSharesInputViewModel input);

        Task<PositionViewModel> GetCurrentPosition(int accountId);
    }
}